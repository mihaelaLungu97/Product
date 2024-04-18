using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ProductUI.Model
{
    public class ProductAPIClient
    {
        private readonly HttpClient _client;

        public ProductAPIClient(string baseUri)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
        }

        public async Task<bool> PostProduceItemAsync(Product item)
        {
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Product", content);

            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Product>> GetProductItemsAsync()
        {
            try
            {
                var response = await _client.GetAsync("/api/Product");
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Product>>(responseData);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching produce items: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> PutProduceItemAsync(Product item)
        {
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/Product/{item.Id}", content);

            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProduceItemAsync(int id)
        {
            var response = await _client.DeleteAsync($"/api/Product/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
