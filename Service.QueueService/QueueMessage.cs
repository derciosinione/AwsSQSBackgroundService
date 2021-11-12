namespace Service.QueueService
{
    public class QueueMessage
    {
        public string MessageId { get; set; }
        public string Body { get; set; } 
        public string ReceiptHandle { get; set; } 
    }
}