using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.QueueService.SQS
{
    public class SqsService : IQueueService
    {
        public async Task<string> GetQueueUrlAsync(string queueName)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> PublishToQueueAsync(string queueUrl, string message)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<QueueMessage>> ReceiveMessageAsync(string queueUrl, int maxMessages = 1)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteMessageAsync(string queueUrl, string id)
        {
            throw new System.NotImplementedException();
        }
    }
}