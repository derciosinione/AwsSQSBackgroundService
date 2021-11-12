using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Service.QueueService;

namespace SQS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AwsSqsController : ControllerBase
    {
        private readonly IQueueService _queueService;
        private readonly IConfiguration _configuration;

        public AwsSqsController(IQueueService queueService, IConfiguration configuration)
        {
            _queueService = queueService;
            _configuration = configuration;
        }
        
        [HttpPost("PublishToQueue")]
        public async Task<ActionResult<bool>> PublishToQueue(string message)
        {
            var queueName = _configuration["AwsSqsConfigurations:QueueName"];
            var queueUrl = await _queueService.GetQueueUrlAsync(queueName);
           
            return  await _queueService.PublishToQueueAsync(queueUrl, message);
        }
    }
}