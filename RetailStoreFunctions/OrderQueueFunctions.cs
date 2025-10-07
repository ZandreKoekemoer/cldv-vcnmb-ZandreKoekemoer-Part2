using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Queues;
using System.Net;

//Reference: Reece Waving. 2025. CLDV6212 Azure functions part 2 Azure functions and queues triggers
// According to Reece Waving (2025), Azure Functions can be triggered by Azure Storage Queues to process messages asynchronously.
// I implemented this approach in the Function to handle the queues for the function.

namespace RetailFunctions
{
    public class OrderQueueFunctions
    {
        private readonly ILogger _logger;
        private readonly string _queueConnection = Environment.GetEnvironmentVariable("AzureStorage")!;
        private readonly string _queueName = "ordersqueue";

        public OrderQueueFunctions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OrderQueueFunctions>();
        }

        [Function("AddOrderToQueue")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders/queue")] HttpRequestData req)
        {
            _logger.LogInformation("AddOrderToQueue function triggered.");

            var requestBody = await req.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                var badResp = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResp.WriteStringAsync("Request body cannot be empty.");
                return badResp;
            }

            try
            {
                var queueClient = new QueueClient(_queueConnection, _queueName);
                await queueClient.CreateIfNotExistsAsync();
                await queueClient.SendMessageAsync(requestBody);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync("Order queued successfully.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to queue.");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("Error sending message to queue.");
                return response;
            }
        }
    }
}
/*
Reece Waving. 2025. CLDV6212 Azure functions part 2 Azure functions and queues triggers (Version 2.0) [Source code].
Available at: <https://youtu.be/zP4umzRCsTM?si=K0Wd3XR2qqF1eDUj>
[Accessed 6 October 2025].
*/
