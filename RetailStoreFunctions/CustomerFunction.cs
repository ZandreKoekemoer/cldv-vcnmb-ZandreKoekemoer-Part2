using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using RetailStoreFunctions.Models;

namespace RetailStoreFunctions
{

    //Reference: Reece Waving. 2025. CLDV6212 Azure Functions Part 1 Getting the basics out the way  HTTP Trigger
    // According to Reece Waving (2025), HTTP triggered Azure Functions can handle table storage operations and return JSON responses.
    // I used this approach in the CustomerFunction to manage customer data in Azure Table Storage with a GET and POST method.

    public class CustomerFunction
    {
        private readonly ILogger<CustomerFunction> _logger;
        private readonly TableClient _tableClient;
        private const string TableName = "Customer";

        public CustomerFunction(ILogger<CustomerFunction> logger)
        {
            _logger = logger;
            var connection = Environment.GetEnvironmentVariable("connection");
            if (string.IsNullOrWhiteSpace(connection))
                throw new InvalidOperationException("Missing storage connection string in environment variable 'connection'.");

            var serviceClient = new TableServiceClient(connection);
            _tableClient = serviceClient.GetTableClient(TableName);
        }

        [Function("GetCustomers")]
        public async Task<HttpResponseData> GetCustomers(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequestData req)
        {
            _logger.LogInformation("GetCustomers called.");

            try
            {
               
                await _tableClient.CreateIfNotExistsAsync();
               
                var customersPageable = _tableClient.QueryAsync<CustomerEntity>();
                var customers = new List<CustomerEntity>();

                await foreach (var customer in customersPageable)
                {
                    customers.Add(customer);
                }

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(customers);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers from table.");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("An error occurred while retrieving customers.");
                return response;
            }
        }


        [Function("AddCustomer")]
        public async Task<HttpResponseData> AddCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequestData req)
        {
            _logger.LogInformation("AddCustomer called.");
            try
            {
                using var sr = new StreamReader(req.Body);
                var body = await sr.ReadToEndAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var customer = JsonSerializer.Deserialize<CustomerEntity>(body, options);
                if (customer == null)
                {
                    var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                    await bad.WriteStringAsync("Invalid customer JSON.");
                    return bad;
                }

                
                customer.PartitionKey = string.IsNullOrEmpty(customer.PartitionKey) ? "CustomerPartition" : customer.PartitionKey;
                customer.RowKey = string.IsNullOrEmpty(customer.RowKey) ? Guid.NewGuid().ToString() : customer.RowKey;

                await _tableClient.CreateIfNotExistsAsync();
                await _tableClient.AddEntityAsync(customer);

                var created = req.CreateResponse(HttpStatusCode.Created);
                await created.WriteAsJsonAsync(customer);
                return created;
            }
            catch (RequestFailedException rfe)
            {
                _logger.LogError(rfe, "Azure Table storage error.");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync($"Storage error: {rfe.Message}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AddCustomer.");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("An unexpected error occurred while adding the customer.");
                return response;
            }
        }
    }
}
/*
Reece Waving. 2025. CLDV6212 Azure Functions Part 1 Getting the basics out the way HTTP Trigger (Version 2.0) [Source code].
Available at: <https://youtu.be/l7s5u-QzYe8?si=spZgBjfEUkeKzfD3>
[Accessed 6 October 2025].
*/
