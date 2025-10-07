using Microsoft.AspNetCore.Mvc;
using MVCRetailStore.Models;
using MVCRetailStore.Services;
using System.Text;
using System.Text.Json;

namespace MVCRetailStore.Controllers
{
    //Reference: Reece Waving. 2025. CLDV6212 Azure functions part 2 Azure functions and queues triggers
    // According to Reece Waving (2025), Azure queue triggers can be used to handle asynchronous messages from applications.
    // I used this approach in the OrderController to send order messages to the Azure queue

    public class OrderController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _functionBase;

        public OrderController(TableStorageService tableStorageService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _tableStorageService = tableStorageService;
            _httpClientFactory = httpClientFactory;
            _functionBase = configuration["FunctionsApi:BaseUrl"]?.TrimEnd('/')
                            ?? throw new ArgumentNullException("FunctionsApi:BaseUrl is missing in configuration.");
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllOrdersAsync();
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();

            ViewData["Customers"] = customers;
            ViewData["Products"] = products;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Customers"] = await _tableStorageService.GetAllCustomersAsync();
                ViewData["Products"] = await _tableStorageService.GetAllProductsAsync();
                return View(order);
            }
            order.PartitionKey = "OrderPartition";
            order.RowKey = Guid.NewGuid().ToString();
            order.orderStatus = "New";
            order.orderDate = DateTime.UtcNow;

            var product = (await _tableStorageService.GetAllProductsAsync())
                          .FirstOrDefault(p => p.RowKey == order.productId);
            if (product != null)
                order.orderTotal = product.Price * Math.Max(1, order.orderQuantity);

            await _tableStorageService.AddOrderAsync(order);
            string message = $"NewOrder|OrderRowKey:{order.RowKey}|CustomerRowKey:{order.customerId}|ProductRowKey:{order.productId}|Qty:{order.orderQuantity}|Date:{order.orderDate:O}";

            try
            {
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent($"\"{message}\"", Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_functionBase}/orders/queue", content);

                if (!response.IsSuccessStatusCode)
                    ModelState.AddModelError("", "Order saved but could not be queued.");
            }
            catch
            {
                ModelState.AddModelError("", "Order saved but failed to send to queue.");
                return View(order);
            }

            TempData["Success"] = "Order created and queued successfully!";
            return RedirectToAction("Index");
        }
    }
}


/*
Reece Waving. 2025.CLDV6212 Azure functions part 2 Azure functions and queues triggers.(Version 2.0) [Source code].
Available at: <https://youtu.be/zP4umzRCsTM?si=K0Wd3XR2qqF1eDUj>
[Accessed 7 October 2025].
*/
