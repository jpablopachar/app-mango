using client.Interfaces;
using client.Models;
using client.Utilities;

namespace client.Services
{
    public class CartService : IShoppingCartService
    {
        private readonly IBaseService _baseService;

        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        /// <summary>Sends a POST request to the shopping cart API to apply a coupon to the cart.</summary>
        /// <param name="CartDto">Represents the cart information.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> ApplyCouponAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.ShoppingCartApi}/api/cart/ApplyCoupon"
            });
        }

        /// <summary>Sends a POST request to the ShoppingCartApi with the provided
        /// `cartDto` data to email the cart.</summary>
        /// <param name="CartDto">Represents a shopping cart.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> EmailCart(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.ShoppingCartApi}/api/cart/EmailCartRequest"
            });
        }

        /// <summary>Retrieves a user's shopping cart by their user ID.</summary>
        /// <param name="userId">Represents the unique identifier of a user.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.ShoppingCartApi}/api/cart/GetCart/{userId}"
            });
        }

        /// <summary>Sends a POST request to the ShoppingCartApi to remove an
        /// item from the cart.</summary>
        /// <param name="cartDetailsId">Represents the unique identifier of the cart details
        /// that you want to remove from the cart.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDetailsId,
                Url = $"{SD.ShoppingCartApi}/api/cart/RemoveCart"
            });
        }

        /// <summary>Sends a POST request to the ShoppingCartApi to upsert a cart
        /// using the provided CartDto.</summary>
        /// <param name="CartDto">Represents a shopping cart.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> UpsertCartAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.ShoppingCartApi}/api/cart/CartUpsert"
            });
        }
    }
}