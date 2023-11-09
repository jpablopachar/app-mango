using shoppingCart_service.Dtos;

namespace shoppingCart_service.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}