using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using product_service.Data;
using product_service.Dtos;
using product_service.Models;

namespace product_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _context;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;

        public ProductController(ProductDbContext context, IMapper mapper)
        {
            _context = context;
            _response = new();
            _mapper = mapper;
        }

        /// <summary>Retrieves a list of products and maps them to a list of
        /// products</summary>
        /// <returns>ResponseDto object.</returns>
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> products = _context.Products.ToList();

                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                _response.Success = false;

                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Retrieves a product by its ID and returns a response containing
        /// the product information.</summary>
        /// <param name="id">Represents the unique identifier of a product.</param>
        /// <returns>ResponseDto object.</returns>
        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product product = _context.Products.First(u => u.ProductId == id);

                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.Success = false;

                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Handles a POST request to add a new product.</summary>
        /// <param name="ProductDto">Represents the product information being passed
        /// in the request body.</param>
        /// <returns>ResponseDto object.</returns>
        [HttpPost]
        // [Authorize(Roles = "ADMIN")]
        public ResponseDto Post(ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);

                _context.Products.Add(product);
                _context.SaveChanges();

                if (productDto.Image != null)
                {
                    string fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    FileInfo file = new(directoryLocation);

                    if (file.Exists) file.Delete();

                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        productDto.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }

                _context.Products.Update(product);
                _context.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Updates a product in the database, including its image if provided, and
        /// returns a response.</summary>
        /// <param name="ProductDto">Represents the product information.</param>
        /// <returns>ResponseDto object.</returns>
        [HttpPut]
        // [Authorize(Roles = "ADMIN")]
        public ResponseDto Put(ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);

                if (productDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);

                        FileInfo file = new FileInfo(oldFilePathDirectory);

                        if (file.Exists) file.Delete();
                    }

                    string fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;

                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        productDto.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }


                _context.Products.Update(product);
                _context.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Deletes a product from a database, including deleting the
        /// associated image file if it exists.</summary>
        /// <param name="id">Represents the unique identifier of the product that needs
        /// to be deleted.</param>
        /// <returns>ResponseDto object.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        // [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Product obj = _context.Products.First(u => u.ProductId == id);

                if (!string.IsNullOrEmpty(obj.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);

                    FileInfo file = new(oldFilePathDirectory);

                    if (file.Exists) file.Delete();
                }

                _context.Products.Remove(obj);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
    }
}