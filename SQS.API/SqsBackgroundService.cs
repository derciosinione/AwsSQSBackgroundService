using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SQS.API
{
    public class SqsBackgroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;

        public SqsBackgroundService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var awsSqsClient = new AmazonSQSClient(RegionEndpoint.USEast1);

            var awsSqsConfigurations = _configuration.GetSection("AwsSqsConfigurations");
            var queueName = awsSqsConfigurations.GetValue<string>("QueueName");
            var queueUrl = await GetQueueUrl(awsSqsClient, queueName);
            
            await Start(awsSqsClient, queueUrl, stoppingToken);
        }

        private async Task Start(AmazonSQSClient sqsClient, string queueUrl, CancellationToken stoppingToken)
        {
            Console.WriteLine($"Starting polling queue at {queueUrl}");

            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await ReceiveMessageAsync(sqsClient, queueUrl);
                await ReadMessageAsync(sqsClient, queueUrl, messages, stoppingToken);
            }
        }

        private static async Task ReadMessageAsync(IAmazonSQS sqsClient, string queueUrl, List<Message> messages, CancellationToken stoppingToken)
        {
            if (messages.Any())
            {
                Console.WriteLine($"{messages.Count} messages received");

                foreach (var msg in messages)
                {
                    var result = ProcessMessage(msg);

                    if (result)
                    {
                        Console.WriteLine($"{msg.MessageId} processed with success");
                        // await DeleteMessageAsync(sqsClient, queueUrl, msg.ReceiptHandle);
                    }
                }
            }
            else
            {
                Console.WriteLine($"---> There is no message available");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        
        private static bool ProcessMessage(Message msg)
        {
            Console.WriteLine($"----> Recived Message {msg.Body}");
            return true;
        }
        
        private static async Task<List<Message>> ReceiveMessageAsync(IAmazonSQS client, string queueUrl)
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                WaitTimeSeconds = 10,
                MaxNumberOfMessages = 10
            };

            var messages = await client.ReceiveMessageAsync(request);
            
            return messages.Messages;
        }

        private static async Task DeleteMessageAsync(IAmazonSQS client, string queueUrl, string messageReceiptHandle)
        {
            await client.DeleteMessageAsync(queueUrl, messageReceiptHandle);
        }
        
        private static async Task<string> GetQueueUrl(IAmazonSQS client, string queueName)
        {
            try
            {
                var response = await client.GetQueueUrlAsync(new GetQueueUrlRequest()
                {
                    QueueName = queueName
                });

                return response.QueueUrl;
            }
            catch (QueueDoesNotExistException)
            {
                Console.WriteLine($"---> The Queue {queueName} does not existis!!!");

                var response = await client.CreateQueueAsync(new CreateQueueRequest()
                {
                    QueueName = queueName
                });
                
                Console.WriteLine($"---> The Queue {queueName} has been created!!!");
                return response.QueueUrl;
            }
            
        }
    }
}