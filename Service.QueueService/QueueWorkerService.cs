using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Service.QueueService
{
    public abstract class QueueWorkerService : BackgroundService
    {
        protected string QueueName { get; set; }
        protected const int MaxMessages  = 10;
        protected const int WaitTimeSeconds  = 15;
        protected int WaitDelayWhenNoMessages { get; set; } = 1;
        
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QueueWorkerService> _logger;
        public QueueWorkerService(IServiceProvider serviceProvider, ILogger<QueueWorkerService> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();

            var queueUrl = await queueService.GetQueueUrlAsync(QueueName);
            
            LogInformation($"Starting polling queue : {QueueName}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var messages = await queueService.ReceiveMessageAsync(queueUrl, WaitTimeSeconds, MaxMessages);
                    await ReadMessageAsync(queueUrl, queueService, messages, stoppingToken);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                }
            }
        }
        
        private async Task ReadMessageAsync(string queueUrl, IQueueService queueService,List<QueueMessage> messages, CancellationToken stoppingToken)
        {
            if (messages.Any())
            {
                LogInformation($"{messages.Count} messages received");

                foreach (var msg in messages)
                {
                    var result = await ProcessMessageAsync(msg);

                    if (result)
                    {
                        LogInformation($"{msg.MessageId} processed with success");
                        await queueService.DeleteMessageAsync( queueUrl, msg.ReceiptHandle);
                    }
                }
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(WaitDelayWhenNoMessages), stoppingToken);
            }
        }
        
        protected abstract Task<bool> ProcessMessageAsync(QueueMessage msg);
        
        protected virtual void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }
        
        protected virtual void LogError(string message)
        {
            _logger.LogError(message);
        }
    }
}