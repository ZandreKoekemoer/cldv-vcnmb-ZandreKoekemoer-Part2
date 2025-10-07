using Azure;
using Azure.Data.Tables;
using System;

namespace RetailStoreFunctions.Models
{
   
    public class CustomerEntity : ITableEntity
    {
        public int? customer_id { get; set; }

        public string? customerName { get; set; }
        public string? password { get; set; }
        public string? customerAddress { get; set; }
        public string? customerPhone { get; set; }
        public string? customerEmail { get; set; }

        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
/*
Reece Waving. 2025. CLDV6212 Azure Functions Part 1 Getting the basics out the way HTTP Trigger (Version 2.0) [Source code].
Available at: <https://youtu.be/l7s5u-QzYe8?si=spZgBjfEUkeKzfD3>
[Accessed 6 October 2025].
*/
