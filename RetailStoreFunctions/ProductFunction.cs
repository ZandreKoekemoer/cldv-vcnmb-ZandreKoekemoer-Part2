using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using RetailStoreFunctions.Models;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace RetailStoreFunctions
{
    //Reference: Reece Waving. 2025. CLDV6212 Azure functions part 4 Azure functions and MVC and blobs
    // According to Reece Waving (2025), Azure Blob Storage operations can be implemented in Azure Functions for serverless file handling.
    // I used this approach in the ProductFunction to upload product images directly to Azure Blob Storage from the Function.

    public class ProductFunction
    {
        private readonly ILogger<ProductFunction> _logger;
        private readonly TableClient _tableClient;
        private readonly BlobContainerClient _blobClient;
        private const string TableName = "Product";
        private const string BlobContainerName = "products";

        public ProductFunction(ILogger<ProductFunction> logger)
        {
            _logger = logger;
            var connection = Environment.GetEnvironmentVariable("connection") ?? throw new System.Exception("Missing storage connection string.");

            
            var serviceClient = new TableServiceClient(connection);
            _tableClient = serviceClient.GetTableClient(TableName);

            
            var blobServiceClient = new BlobServiceClient(connection);
            _blobClient = blobServiceClient.GetBlobContainerClient(BlobContainerName);
        }

        [Function("GetProducts")]
        public async Task<HttpResponseData> GetProducts(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequestData req)
        {
            _logger.LogInformation("GetProducts called.");

            try
            {
                
                await _tableClient.CreateIfNotExistsAsync();

                var productsPageable = _tableClient.QueryAsync<ProductEntity>();
                var products = new List<ProductEntity>();

                await foreach (var product in productsPageable)
                {
                    products.Add(product);
                }

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(products);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products from table.");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("An error occurred while retrieving products.");
                return response;
            }
        }

        [Function("AddProduct")]
        public async Task<HttpResponseData> AddProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
        {
            _logger.LogInformation("AddProduct called.");

            try
            {
                var multipart = req.Headers.GetValues("Content-Type").FirstOrDefault()?.Split(';');
                if (multipart == null || !multipart.Any())
                    throw new System.Exception("Invalid multipart content.");

                var reader = new MultipartReader(multipart[1].Split('=')[1].Trim(), req.Body);
                var section = await reader.ReadNextSectionAsync();

                ProductEntity product = new ProductEntity();
                Stream? fileStream = null;
                string? fileName = null;

                while (section != null)
                {
                    var contentDisposition = section.Headers["Content-Disposition"].ToString();
                    if (contentDisposition.Contains("form-data"))
                    {
                        var name = contentDisposition.Split(';').First(x => x.Trim().StartsWith("name="))
                                                    .Split('=')[1].Trim('\"');

                        if (name == "file")
                        {
                            fileStream = section.Body;
                            fileName = contentDisposition.Split(';').First(x => x.Trim().StartsWith("filename="))
                                                        .Split('=')[1].Trim('\"');
                        }
                        else
                        {
                            using var sr = new StreamReader(section.Body);
                            var value = await sr.ReadToEndAsync();
                            switch (name)
                            {
                                case "ProductName": product.ProductName = value; break;
                                case "Description": product.Description = value; break;
                                case "Price": product.Price = double.TryParse(value, out double p) ? p : 0; break;
                            }
                        }
                    }
                    section = await reader.ReadNextSectionAsync();
                }
                if (fileStream != null && !string.IsNullOrEmpty(fileName))
                {
                    await _blobClient.CreateIfNotExistsAsync();
                    var blob = _blobClient.GetBlobClient(fileName);
                    await blob.UploadAsync(fileStream, overwrite: true);
                    product.ImageUrl = blob.Uri.ToString();
                }
                product.PartitionKey = "ProductPartition";
                product.RowKey = Guid.NewGuid().ToString();
                await _tableClient.CreateIfNotExistsAsync();
                await _tableClient.AddEntityAsync(product);

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(product);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in AddProduct");
                var resp = req.CreateResponse(HttpStatusCode.InternalServerError);
                await resp.WriteStringAsync($"Error: {ex.Message}");
                return resp;
            }
        }
    }
}
/*
Reece Waving. 2025. CLDV6212 Azure functions part 4 Azure functions and MVC and blobs (Version 2.0) [Source code].
Available at: <https://youtu.be/r-VksPFfFpE?si=mS1bzTpQk9wf5xZn>
[Accessed 6 October 2025].
*/
