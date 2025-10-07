using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MVCRetailStore.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MVCRetailStore.Controllers
{
    //Reference: Reece Waving. 2025. CLDV6212 Azure functions part 4 Azure functions and MVC and blobs
    // According to Reece Waving (2025), Azure Blob Storage can be integrated with MVC controllers to handle file uploads and downloads.
    // I used this approach in the ProductController to upload product images to Azure Blob Storage.

    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBase;

        public ProductController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBase = configuration["FunctionsApi:BaseUrl"]?.TrimEnd('/')
                ?? throw new System.ArgumentNullException("FunctionsApi:BaseUrl is missing in appsettings.");
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync($"{_apiBase}/products");
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                var products = await JsonSerializer.DeserializeAsync<IEnumerable<Product>>(stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Product>();

                return View(products);
            }
            catch
            {
                ViewBag.ErrorMessage = "Unable to retrieve products";
                return View(new List<Product>());
            }
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? file)
        {
            var client = _httpClientFactory.CreateClient();
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(product.ProductName ?? ""), "ProductName");
            form.Add(new StringContent(product.Description ?? ""), "Description");
            form.Add(new StringContent(product.Price.ToString()), "Price");

            if (file != null && file.Length > 0)
                form.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

            await client.PostAsync($"{_apiBase}/products", form);

            TempData["Success"] = "Product added successfully!";
            return RedirectToAction("Index");
        }
    }
}



/*
Reece Waving. 2025. CLDV6212 Azure functions part 4 Azure functions and MVC and blobs (Version 2.0) [Source code].
Available at: <https://youtu.be/r-VksPFfFpE?si=mS1bzTpQk9wf5xZn>
[Accessed 6 October 2025].
*/
