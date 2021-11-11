# SQSDemoBackgroundService

The scope of this article it to get you started building a SQS background services using .NET 5 (should work with 3.1 as well).
SQS stands for Simple Queue Service. That is, it’s a queue service. What you can do with it is pretty straightforward.
You create a queue
You put messages in the queue
You read messages from the queue
You process the messages
On success, you remove the messages from the queue
What happens when something fails? Well, the message stay in the queue for reprocessing until future success. It means that you can use SQS to easily build asynchronous resilient cross platform applications.
