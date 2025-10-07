using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace MVCRetailStore.Models
{
    public class Customer : ITableEntity
    {
        [Key]
        public int customer_id { get; set; }
        //Displays an error msg if no name is entered

        [Required(ErrorMessage = "Customer Name is required")]
        public string? customerName { get; set; }
        //Error msg when no pasword is entered
        [Required(ErrorMessage = "Password is required")]
        public string? password { get; set; }
        public string? customerAddress { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? customerPhone { get; set; }
        //customer email is required to continue
        [Required(ErrorMessage = "Email is required")]
        public string? customerEmail { get; set; }

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


Stack Overflow. 2015. Calculate price based on input number (quantity) change.  
Available at: <https://stackoverflow.com/questions/27764823/calculate-price-based-on-input-number-quantity-change>  
[Accessed 28 August 2025].

*/