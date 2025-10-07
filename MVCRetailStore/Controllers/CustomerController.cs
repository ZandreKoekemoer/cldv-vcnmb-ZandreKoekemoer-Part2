using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MVCRetailStore.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MVCRetailStore.Controllers
{
    //Reference: Reece Waving. 2025. CLDV6212 Azure Functions Part 1 Getting the basics out the way  HTTP Trigger
    // According to Reece Waving (2025), HTTP triggered Azure Functions can be called from MVC controller.
    // I used this approach in the CustomerController to retrieve and create customer records with Azure Functions.

    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBase;

        public CustomerController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBase = configuration["FunctionsApi:BaseUrl"]?.TrimEnd('/')
                ?? throw new ArgumentNullException("FunctionsApi:BaseUrl is missing in appsettings.");
        }
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBase}/customers");
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var customers = await JsonSerializer.DeserializeAsync<IEnumerable<Customer>>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Customer>();

            return View(customers);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid) return View(customer);

            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(customer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_apiBase}/customers", content);
            response.EnsureSuccessStatusCode();

            TempData["Success"] = "Customer created successfully";
            return RedirectToAction("Index");
        }
    }
}
/*
Reece Waving. 2025. CLDV6212 Azure Functions Part 1 Getting the basics out the way HTTP Trigger (Version 2.0) [Source code].
Available at: <https://youtu.be/l7s5u-QzYe8?si=spZgBjfEUkeKzfD3>
[Accessed 6 October 2025].
*/
