using System;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;

namespace Service.QueueService.SQS
{
    public static class Extensions
    {
        public static IServiceCollection AddSqsService(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonSQS>();
            services.AddScoped<IQueueService, SqsService>();

            return services;
        }
    }
}