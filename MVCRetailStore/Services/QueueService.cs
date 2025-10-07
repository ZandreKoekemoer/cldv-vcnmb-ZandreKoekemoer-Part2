using Azure.Storage.Queues;
using System.Text;

//Reference: Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 3: Never Lose Data Again with Queue Storage! (Version 2.0) [Source code].
// According to Reece Waving (2025), messages can be sent to Azure Queue Storage using the QueueClient's SendMessageAsync method.
// I used this reference since it helped me set a guideline on creating a queue service.
namespace MVCRetailStore.Services
{
    public class QueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService(string connectionString, string queueName)
        {
            _queueClient = new QueueClient(connectionString, queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessage(string message)
        {
            
            await _queueClient.SendMessageAsync(message);
        }
    }
}

/*
Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 3: Never Lose Data Again with Queue Storage!(Version 2.0) [Source code].
Available at: <https://youtu.be/VbZ3Pi63yEc?si=LQjhLWhylEcbOl7z>
[Accessed 28 August 2025].
*/