using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SQSDemoBackgroundService
{
    public class SqsBackGroundService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new System.NotImplementedException();
        }
    }
}