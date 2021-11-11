using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SQSDemoBackgroundService
{
    public class SqsBackGroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;

        public SqsBackGroundService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var awsSqsClient = new AmazonSQSClient(RegionEndpoint.USEast1);

            var awsSqsConfigurations = _configuration.GetSection("AwsSqsConfigurations");
            var queueName = awsSqsConfigurations.GetValue<string>("QueueName");
            var queueUrl = await GetQueueUrl(awsSqsClient, queueName);
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
                var response = await client.CreateQueueAsync(new CreateQueueRequest()
                {
                    QueueName = queueName
                });

                return response.QueueUrl;
            }
        }
    }
}