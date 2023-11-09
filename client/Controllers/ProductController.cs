using client.Interfaces;
using client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace client.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>Retrieves a list of products from a service and returns a view with the
        /// products.</summary>
        /// <returns>Returning a ViewResult, which represents a view that will
        /// be rendered to the client.</returns>
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? products = new();

            ResponseDto response = await _productService.GetAllProductsAsync();

            if (response != null && response.Success)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response!.Result)!);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(products);
        }

        /// <summary>Returns a view for creating a product.</summary>
        /// <returns>View result.</returns>
        public IActionResult ProductCreate()
        {
            return View();
        }

        /// <summary>Handles the creation of a product and redirects to the product
        /// index page if successful.</summary>
        /// <param name="ProductDto">Represents the data needed to create a
        /// product.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await _productService.CreateProductAsync(productDto);

                if (response != null && response.Success)
                {
                    TempData["success"] = "Producto creado correctamente.";

                    return RedirectToAction(nameof(ProductIndex));
                }

                TempData["error"] = response?.Message;
            }

            return View(productDto);
        }

        /// <summary>Returns the product view if successful, otherwise returns a not
        /// found error.</summary>
        /// <param name="productId">Represents the unique identifier of the product that needs to be deleted.</param>
        /// <returns>IActionResult.</returns>
        public async Task<IActionResult> ProductDelete(int productId)
        {
            ResponseDto response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.Success)
            {
                ProductDto? product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result)!);

                return View(product);
            }

            TempData["error"] = response?.Message;

            return NotFound();
        }

        /// <summary>Handles the deletion of a product and redirects to the product index page if successful.</summary>
        /// <param name="ProductDto">Represents a product. It contains properties such as ProductId, which is the unique identifier of the product.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto productDto)
        {
            ResponseDto response = await _productService.DeleteProductAsync(productDto.ProductId);

            if (response != null && response.Success)
            {
                TempData["success"] = "Producto eliminado correctamente.";

                return RedirectToAction(nameof(ProductIndex));
            }

            TempData["error"] = response?.Message;

            return View(productDto);
        }

        /// <summary>Retrieves a product by its ID, deserializes it, and returns the product view if successful, otherwise returns a not found error.</summary>
        /// <param name="productId">Represents the unique identifier of a product.</param>
        /// <returns>IActionResult.</returns>
        public async Task<IActionResult> ProductEdit(int productId)
        {
            ResponseDto response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.Success)
            {
                ProductDto product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result)!)!;

                return View(product);
            }

            TempData["error"] = response?.Message;

            return NotFound();
        }

        /// <summary>Handles the HTTP POST request for editing a product, updating it
        /// in the database, and redirecting to the product index page.</summary>
        /// <param name="ProductDto">Represents the product information.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await _productService.UpdateProductAsync(productDto);

                if (response != null && response.Success)
                {
                    TempData["success"] = "Producto actualizado correctamente.";

                    return RedirectToAction(nameof(ProductIndex));
                }

                TempData["error"] = response?.Message;
            }

            return View(productDto);
        }
    }
}