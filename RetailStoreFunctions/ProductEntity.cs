using Azure;
using Azure.Data.Tables;
using System;

namespace RetailStoreFunctions.Models
{
    public class ProductEntity : ITableEntity
    {
        public int? ProductId { get; set; }

        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
/*
Reece Waving. 2025. CLDV6212 Azure functions part 4 Azure functions and MVC and blobs (Version 2.0) [Source code].
Available at: <https://youtu.be/r-VksPFfFpE?si=mS1bzTpQk9wf5xZn>
[Accessed 6 October 2025].
*/
