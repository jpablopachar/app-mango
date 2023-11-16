using order_service.Dtos;

namespace order_service.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}