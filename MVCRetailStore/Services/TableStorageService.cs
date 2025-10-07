using Azure;
using Azure.Data.Tables;
using MVCRetailStore.Models;

//Reference: Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 2: Adding Image Uploads with Blob Storage.
// According to Reece Waving (2025), image files can be uploaded to Azure Blob Storage by opening a file stream and sending it via the BlobService client.
// I used this reference to understand the structure and how TableStorage is being used.

namespace MVCRetailStore.Services
{
    public class TableStorageService
    {
        private readonly TableClient _customerTableClient;
        private readonly TableClient _productTableClient;
        private readonly TableClient _orderTableClient;

        public TableStorageService(string connectionString)
        {
            _customerTableClient = new TableClient(connectionString, "customer");
            _productTableClient = new TableClient(connectionString, "product");
            _orderTableClient = new TableClient(connectionString, "order");

            _customerTableClient.CreateIfNotExists();
            _productTableClient.CreateIfNotExists();
            _orderTableClient.CreateIfNotExists();
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();
            await foreach (var c in _customerTableClient.QueryAsync<Customer>())
            {
                customers.Add(c);
            }
            return customers;
        }

        public async Task<Customer?> GetCustomerAsync(string partitionKey, string rowKey)
        {
            try
            {
                return (await _customerTableClient.GetEntityAsync<Customer>(partitionKey, rowKey)).Value;
            }
            catch
            {
                return null;
            }
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.PartitionKey) || string.IsNullOrEmpty(customer.RowKey))
                throw new ArgumentException("PartitionKey and RowKey must be set.");

            await _customerTableClient.AddEntityAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _customerTableClient.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);
        }

        public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
        {
            await _customerTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var p in _productTableClient.QueryAsync<Product>())
            {
                products.Add(p);
            }
            return products;
        }

        public async Task<Product?> GetProductAsync(string partitionKey, string rowKey)
        {
            try
            {
                return (await _productTableClient.GetEntityAsync<Product>(partitionKey, rowKey)).Value;
            }
            catch
            {
                return null;
            }
        }

        public async Task AddProductAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.PartitionKey) || string.IsNullOrEmpty(product.RowKey))
                throw new ArgumentException("PartitionKey and RowKey must be set.");

            await _productTableClient.AddEntityAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productTableClient.UpdateEntityAsync(product, ETag.All, TableUpdateMode.Replace);
        }

        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await _productTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = new List<Order>();
            await foreach (var o in _orderTableClient.QueryAsync<Order>())
            {
                orders.Add(o);
            }
            return orders;
        }

        public async Task<Order?> GetOrderAsync(string partitionKey, string rowKey)
        {
            try
            {
                return (await _orderTableClient.GetEntityAsync<Order>(partitionKey, rowKey)).Value;
            }
            catch
            {
                return null;
            }
        }

        public async Task AddOrderAsync(Order order)
        {
            if (string.IsNullOrEmpty(order.PartitionKey) || string.IsNullOrEmpty(order.RowKey))
                throw new ArgumentException("PartitionKey and RowKey must be set.");

            await _orderTableClient.AddEntityAsync(order);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderTableClient.UpdateEntityAsync(order, ETag.All, TableUpdateMode.Replace);
        }

        public async Task DeleteOrderAsync(string partitionKey, string rowKey)
        {
            await _orderTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}

/*
Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 2: Adding Image Uploads with Blob Storage! (Version 2.0) [Source code].
Available at: <https://youtu.be/CuszKqZvRuM?si=lTfJaqI02wmHcIkh>
[Accessed 26 August 2025].
*/