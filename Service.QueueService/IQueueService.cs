using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.QueueService
{
    public interface IQueueService
    {
        Task<string> GetQueueUrlAsync(string queueName);
        
        Task<bool> PublishToQueueAsync(string queueUrl, string message);
        
        Task<List<QueueMessage>> ReceiveMessageAsync(string queueUrl, int waitTimeSeconds = 0, int maxMessages = 1);

        Task DeleteMessageAsync(string queueUrl, string id);
    }
}