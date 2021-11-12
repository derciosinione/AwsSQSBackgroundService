using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Service.QueueService
{
    public class QueueWorkerService : BackgroundService
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
            throw new NotImplementedException();
        }
    }
}