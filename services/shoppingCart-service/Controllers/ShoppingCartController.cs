using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shoppingCart_service.Data;
using shoppingCart_service.Dtos;
using shoppingCart_service.Interfaces;
using shoppingCart_service.Models;

namespace shoppingCart_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController
    {
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly ShoppingCartDbContext _context;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;

        public ShoppingCartController(IMapper mapper, ShoppingCartDbContext context, IProductService productService, ICouponService couponService)
        {
            _response = new();
            _mapper = mapper;
            _context = context;
            _productService = productService;
            _couponService = couponService;
        }

        /// <summary>Retrieves a user's cart information.</summary>
        /// <param name="userId">Represents the unique identifier of a user.</param>
        /// <returns>`ResponseDto` object.</returns>
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_context.CartHeaders.First(u => u.UserId == userId)),
                };

                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_context.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> products = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = products.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += item.Count * item.Product!.Price;
                }

                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);

                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Updates the coupon code in the cart header and saves the changes
        /// to the database.</summary>
        /// <param name="CartDto">Represents the cart information.</param>
        /// <returns>`Task<object>`.</returns>
        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _context.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader!.UserId);

                cartFromDb.CouponCode = cartDto.CartHeader!.CouponCode;

                _context.CartHeaders.Update(cartFromDb);

                await _context.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.ToString();
            }

            return _response;
        }

        /// <summary>Handles the insertion or update of cart data in a database.</summary>
        /// <param name="CartDto">Contains information about a cart.</param>
        /// <returns>`ResponseDto` object.</returns>
        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader!.UserId);

                if (cartHeaderFromDb == null)
                {
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);

                    _context.CartHeaders.Add(cartHeader);

                    await _context.SaveChangesAsync();

                    cartDto.CartDetails!.First().CartHeaderId = cartHeader.CartHeaderId;

                    _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails!.First()));

                    await _context.SaveChangesAsync();
                }
                else
                {
                    var cartDetailsFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetails!.First().ProductId && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if (cartDetailsFromDb == null)
                    {
                        cartDto.CartDetails!.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;

                        _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails!.First()));

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        cartDto.CartDetails!.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails!.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails!.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;

                        _context.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails!.First()));

                        await _context.SaveChangesAsync();
                    }
                }

                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.Success = false;
            }

            return _response;
        }

        /// <summary>Removes a cart item from the database and also removes the cart
        /// header if it was the last item in the cart. </summary>
        /// <param name="cartDetailsId">Represents the unique identifier of a specific
        /// cart detail item.</param>
        /// <returns>`ResponseDto` object.</returns>
        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _context.CartDetails.First(u => u.CartDetailsId == cartDetailsId);

                int totalCountOfCartItem = _context.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();

                _context.CartDetails.Remove(cartDetails);

                if (totalCountOfCartItem == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove!);
                }

                await _context.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.Success = false;
            }

            return _response;
        }
    }
}