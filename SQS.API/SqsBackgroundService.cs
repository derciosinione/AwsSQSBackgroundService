
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.QueueService;

namespace SQS.API
{
    public class SqsBackgroundService : QueueWorkerService
    {
        public SqsBackgroundService(IServiceProvider serviceProvider, ILogger<QueueWorkerService> logger, 
                                    IConfiguration configuration) : base(serviceProvider, logger)
        {
            QueueName = configuration["AwsSqsConfigurations:QueueName"];
        }

        protected override async Task<bool> ProcessMessageAsync(QueueMessage msg)
        {
            LogInformation($"---> API Recived : {msg.Body}");
            return await Task.FromResult(true);
        }
    }
}