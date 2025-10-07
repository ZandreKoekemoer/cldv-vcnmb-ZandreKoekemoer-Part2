using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace MVCRetailStore.Models
{
    public class Order : ITableEntity
    {
        [Key] 
        public int OrderId { get; set; }
        //If user does not enter the amount of the product he chooses it will display an error msg
        [Required(ErrorMessage = "Quantity is required")]
        public int orderQuantity { get; set; }

        public double orderTotal { get; set; }

        // When creating an order the user has to choose a customer 
        [Required(ErrorMessage = "Please select a Customer")]
        public string? customerId { get; set; } 
        //User has to also choose a product
        [Required(ErrorMessage = "Please select a Product")]
        public string? productId { get; set; } 

        public DateTime orderDate { get; set; }

        public string? orderStatus { get; set; }

        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
/*
Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 3: Never Lose Data Again with Queue Storage!(Version 2.0) [Source code].
Available at: <https://youtu.be/VbZ3Pi63yEc?si=LQjhLWhylEcbOl7z>
[Accessed 28 August 2025].

/*
Stack Overflow. 2015. MVC - Data Calculations Best Practice - ViewModel vs. Controller. [online]
Available at: <https://stackoverflow.com/questions/31730642/mvc-data-calculations-best-practice-viewmodel-vs-controller>
[Accessed 28 August 2025].


*/