using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Service.QueueService.SQS
{
    public class SqsService : IQueueService
    {
        private readonly IAmazonSQS _awsSqsClient;

        public SqsService(IAmazonSQS awsSqsClient)
        {
            _awsSqsClient = awsSqsClient;
        }
        public async Task<string> GetQueueUrlAsync(string queueName)
        {
            try
            {
                var response = await _awsSqsClient.GetQueueUrlAsync(new GetQueueUrlRequest()
                {
                    QueueName = queueName
                });

                return response.QueueUrl;
            }
            catch (QueueDoesNotExistException)
            {
                Console.WriteLine($"---> The Queue {queueName} does not existis!!!");

                var response = await _awsSqsClient.CreateQueueAsync(new CreateQueueRequest()
                {
                    QueueName = queueName
                });
                
                Console.WriteLine($"---> The Queue {queueName} has been created!!!");
                return response.QueueUrl;
            }

        }

        public async Task<bool> PublishToQueueAsync(string queueUrl, string message)
        {
            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = message
            };
            
            var response = await _awsSqsClient.SendMessageAsync(request);

            return response.HttpStatusCode == HttpStatusCode.OK;
        }

        public async Task<List<QueueMessage>> ReceiveMessageAsync(string queueUrl, int waitTimeSeconds = 0, int maxMessages = 1)
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                WaitTimeSeconds = 10,
                MaxNumberOfMessages = maxMessages
            };
            
            var messages = await _awsSqsClient.ReceiveMessageAsync(request);

            return messages.Messages.Select(m => new QueueMessage
            {
                MessageId = m.MessageId,
                Body = m.Body,
                ReceiptHandle = m.ReceiptHandle
            }).ToList();
        }

        public async Task DeleteMessageAsync(string queueUrl, string id)
        {
            await _awsSqsClient.DeleteMessageAsync(queueUrl, id);
        }
    }
}