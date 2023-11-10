using System.IdentityModel.Tokens.Jwt;
using client.Interfaces;
using client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace client.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        /// <summary>Removes an item from the shopping cart and redirects to the cart index
        /// page if successful.</summary>
        /// <param name="cartDetailsId">Represents the ID of the cart details item that needs
        /// to be removed from the shopping cart.</param>
        /// <returns>If the response is not null and the response's success property is
        /// true, then a redirect to the "CartIndex" action is returned. Otherwise, a view
        /// is returned.</returns>
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()!.Value;

            ResponseDto response = await _shoppingCartService.RemoveFromCartAsync(cartDetailsId);

            if (response != null && response.Success)
            {
                TempData["success"] = "Carrito actualizado correctamente.";

                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        /// <summary>Handles the HTTP POST request to apply a coupon to the shopping cart and
        /// redirects to the cart index page if successful.</summary>
        /// <param name="CartDto">Represents the shopping cart.</param>
        /// <returns>If the response is not null and the response's Success property is true,
        /// then the method will return a RedirectToAction to the CartIndex action.
        /// Otherwise, it will return a View.</returns>
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDto response = await _shoppingCartService.ApplyCouponAsync(cartDto);

            if (response != null && response.Success)
            {
                TempData["success"] = "Carrito actualizado correctamente.";

                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        /// <summary>Handles the process of emailing the contents of a shopping cart to
        /// the user.</summary>
        /// <param name="CartDto">Represents a shopping cart.</param>
        /// <returns>If the `response` is not null and `response.Success` is true, then the
        /// method will return a `RedirectToAction` to the `CartIndex` action. Otherwise, it
        /// will return a `View`.</returns>
        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cartDtoFromDb = await LoadCartDtoBasedOnLoggedInUser();

            cartDto.CartHeader!.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email).FirstOrDefault()!.Value;

            ResponseDto response = await _shoppingCartService.EmailCart(cartDtoFromDb);

            if (response != null && response.Success)
            {
                TempData["success"] = "El correo electrónico será procesado y enviado en breve";

                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        /// <summary>Removes a coupon code from the cart and redirects to the cart
        /// index page if the coupon removal is successful.</summary>
        /// <param name="CartDto">Represents the shopping cart.</param>
        /// <returns>If the response is successful, the method will redirect to the
        /// "CartIndex" action. If the response is not successful or null, the method will
        /// return a View.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader!.CouponCode = "";

            ResponseDto response = await _shoppingCartService.ApplyCouponAsync(cartDto);

            if (response != null && response.Success)
            {
                TempData["success"] = "Cupón removido correctamente.";

                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        /// <summary>Loads a `CartDto` object based on the currently logged-in user.</summary>
        /// <returns>Task object that represents the asynchronous operation of loading
        /// a CartDto based on the logged-in user.</returns>
        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()!.Value;

            ResponseDto response = await _shoppingCartService.GetCartByUserIdAsync(userId);

            if (response != null && response.Success) return JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result)!)!;

            return new CartDto();
        }
    }
}