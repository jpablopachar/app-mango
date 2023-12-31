using Newtonsoft.Json;
using shoppingCart_service.Dtos;
using shoppingCart_service.Interfaces;

namespace shoppingCart_service.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>Sends an HTTP GET request to a product API, retrieves the response, and
        /// deserializes it into a list of `ProductDto` objects.</summary>
        /// <returns>`IEnumerable<ProductDto>`.</returns>
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if (resp!.Success) return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result)!)!;

            return new List<ProductDto>();
        }
    }
}