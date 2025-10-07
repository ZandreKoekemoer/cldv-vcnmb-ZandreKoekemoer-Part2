using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace MVCRetailStore.Models
{
    public class Product : ITableEntity
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        public string? ProductName { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; }
        public string? ImageUrl { get; set; }

        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
/*
Bootstrap. 2023. Bootstrap 5 Documentation.
Available at:
<https: //getbootstrap.com/docs/5.3/getting-started/introduction />
[Accessed 28 August 2025].

/*
Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 3: Never Lose Data Again with Queue Storage!(Version 2.0) [Source code].
Available at: <https://youtu.be/VbZ3Pi63yEc?si=LQjhLWhylEcbOl7z>
[Accessed 28 August 2025].

*/