using Microsoft.AspNetCore.Mvc;
using MVCRetailStore.Models;
using System.Text.Json;

namespace MVCRetailStore.Controllers
{
    public class FilesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBase;

        public FilesController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBase = configuration["FunctionsApi:BaseUrl"]?.TrimEnd('/');
        }
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var files = new List<FileModel>();

            try
            {
                var response = await client.GetAsync($"{_apiBase}/files");
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<List<FileModel>>(stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data != null)
                    files = data;
            }
            catch
            {
                ViewBag.Message = "Could not load files.";
            }

            return View(files);
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Message"] = "Please select a file.";
                return RedirectToAction("Index");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var client = _httpClientFactory.CreateClient();

                var url = $"{_apiBase}/files/upload?fileName={Uri.EscapeDataString(file.FileName)}";
                var content = new StreamContent(stream);
                content.Headers.Add("Content-Type", "application/octet-stream");

                var response = await client.PostAsync(url, content);
                TempData["Message"] = response.IsSuccessStatusCode
                    ? $"File '{file.FileName}' uploaded successfully."
                    : $"Upload failed: {response.ReasonPhrase}";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Upload error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}



/*
Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 2: Adding Image Uploads with Blob Storage! (Version 2.0) [Source code].
Available at: <https://youtu.be/CuszKqZvRuM?si=lTfJaqI02wmHcIkh>
[Accessed 26 August 2025].


Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 4: Mastering Azure File Share! (Version 2.0) [Source code].
Available at: <https://youtu.be/A-mVVL88oEg?si=sUYFyrY2wQc6Lny0>
[Accessed 26 August 2025].

*/