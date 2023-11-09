using client.Interfaces;
using client.Models;
using client.Utilities;

namespace client.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;

        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        /// <summary>Sends a POST request to the product API with the provided
        /// product data.</summary>
        /// <param name="ProductDto">Contains the information of a product.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> CreateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = productDto,
                Url = $"{SD.ProductApi}/api/product",
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        /// <summary>Sends a DELETE request to the specified URL to delete a
        /// product with the given ID.</summary>
        /// <param name="id">Is the unique identifier of the product that needs to be
        /// deleted.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> DeleteProductAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.ProductApi}/api/product/{id}"
            });
        }

        /// <summary>Sends a GET request to the specified URL and returns a
        /// `ResponseDto`.</summary>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> GetAllProductsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.ProductApi}/api/product"
            });
        }

        /// <summary>Sends a GET request to the specified URL to retrieve a
        /// product by its ID.</summary>
        /// <param name="id">Represents the unique identifier of the product you want
        /// to retrieve.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.ProductApi}/api/product/{id}"
            });
        }

        /// <summary>Sends a PUT request to update a product using the `_baseService`
        /// and returns a `ResponseDto`.</summary>
        /// <param name="ProductDto">Contains the information of a product.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> UpdateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = productDto,
                Url = $"{SD.ProductApi}/api/product",
                ContentType = SD.ContentType.MultipartFormData
            });
        }
    }
}