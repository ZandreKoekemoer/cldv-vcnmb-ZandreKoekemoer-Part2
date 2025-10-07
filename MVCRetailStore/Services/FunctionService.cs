using MVCRetailStore.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MVCRetailStore.Services
{
    public class FunctionService
    {
        private readonly HttpClient _http;

        // HttpClient is injected via typed client in Program.cs
        public FunctionService(HttpClient http)
        {
            _http = http;
        }

        // 🔹 POST - Add new customer
        public async Task<bool> AddCustomerAsync(Customer customer)
        {
            var json = JsonSerializer.Serialize(customer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("customers/add", content);
            return response.IsSuccessStatusCode;
        }

        // 🔹 GET - Retrieve all customers
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var response = await _http.GetAsync("customers/get");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Customer>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Customer>();
        }
    }
}
