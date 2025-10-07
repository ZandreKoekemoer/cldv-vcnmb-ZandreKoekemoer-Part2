using MVCRetailStore.Services;
using System.Globalization;

namespace MVCRetailStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Set culture info for consistent number formatting
            var cultureInfo = new CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            // Get configuration values
            var storageConn = builder.Configuration.GetConnectionString("AzureStorage")!;
            var blobContainer = builder.Configuration["Storage:BlobContainer"] ?? "products";
            var queueName = builder.Configuration["Storage:QueueName"] ?? "ordersqueue";
            var fileShare = builder.Configuration["Storage:FileShare"] ?? "contracts";

            // Register services
            builder.Services.AddSingleton(new TableStorageService(storageConn));
            builder.Services.AddSingleton(new BlobService(storageConn, blobContainer));
            builder.Services.AddSingleton(new QueueService(storageConn, queueName));
            builder.Services.AddSingleton(new AzureFileShareService(storageConn, fileShare));
            builder.Services.AddHttpClient();
            // Register FunctionService with HttpClient and BaseAddress
           
            var app = builder.Build();

            // Configure middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{partitionKey?}/{rowKey?}");

            app.Run();
        }
    }
}


/*
Bootstrap. 2023. Bootstrap 5 Documentation.
Available at:
<https: //getbootstrap.com/docs/5.3/getting-started/introduction />
[Accessed 28 August 2025].
*/