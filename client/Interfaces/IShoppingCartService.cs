using client.Models;

namespace client.Interfaces
{
    public interface IShoppingCartService
    {
        Task<ResponseDto> GetCartByUserIdAsync(string userId);
        Task<ResponseDto> UpsertCartAsync(CartDto cartDto);
        Task<ResponseDto> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDto> ApplyCouponAsync(CartDto cartDto);
        Task<ResponseDto> EmailCart(CartDto cartDto);
    }
}