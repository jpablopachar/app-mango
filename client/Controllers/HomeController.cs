using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using client.Models;
using client.Interfaces;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace client.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;

    public HomeController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDto> products = new();

        ResponseDto response = await _productService.GetAllProductsAsync();

        if (response != null && response.Success)
        {
            products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response!.Result)!)!;
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(products);
    }

    [Authorize]
    public async Task<IActionResult> ProductDetails(int productId)
    {
        ProductDto product = new();

        ResponseDto response = await _productService.GetProductByIdAsync(productId);

        if (response != null && response.Success)
        {
            product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result)!)!;
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(product);
    }


    /* [Authorize]
    [HttpPost]
    [ActionName("ProductDetails")]
    public async Task<IActionResult> ProductDetails(ProductDto productDto)
    {
        CartDto cartDto = new CartDto()
        {
            CartHeader = new CartHeaderDto
            {
                UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
            }
        };

        CartDetailsDto cartDetails = new CartDetailsDto()
        {
            Count = productDto.Count,
            ProductId = productDto.ProductId,
        };

        List<CartDetailsDto> cartDetailsDtos = new() { cartDetails };
        cartDto.CartDetails = cartDetailsDtos;

        ResponseDto? response = await _cartService.UpsertCartAsync(cartDto);

        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Item has been added to the Shopping Cart";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(productDto);
    } */

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
