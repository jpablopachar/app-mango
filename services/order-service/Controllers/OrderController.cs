using AutoMapper;
using message_bus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using order_service.Data;
using order_service.Dtos;
using order_service.Interfaces;
using order_service.Models;
using Microsoft.EntityFrameworkCore;
using order_service.Utilities;
using Stripe.Checkout;
using Stripe;

namespace order_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly OrderDbContext _orderDbContext;
        private readonly IProductService _productService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public OrderController(IMapper mapper, OrderDbContext orderDbContext, IProductService productService, IMessageBus messageBus, IConfiguration configuration)
        {
            _response = new();
            _mapper = mapper;
            _orderDbContext = orderDbContext;
            _productService = productService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        /// <summary>Retrieves a list of orders based on the user's role and user
        /// ID.</summary>
        /// <param name="userId">Represents the user ID. It is used to filter the
        /// orders based on the user ID.</param>
        /// <returns>ResponseDto object.</returns>
        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDto Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> orders;

                if (User.IsInRole(SD.ROLE_ADMIN))
                {
                    orders = _orderDbContext.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                else
                {
                    orders = _orderDbContext.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.OrderHeaderId).ToList();
                }

                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(orders);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Retrieves an order with a specific ID and returns it as a
        /// ResponseDto object.</summary>
        /// <param name="id">Represents the unique identifier of an order.</param>
        /// <returns>ResponseDto object.</returns>
        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDto? Get(int id)
        {
            try
            {
                OrderHeader orderHeader = _orderDbContext.OrderHeaders.Include(u => u.OrderDetails).First(u => u.OrderHeaderId == id);

                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Creates an order based on the provided cart data and returns a
        /// response.</summary>
        /// <param name="CartDto">Represents a shopping cart.</param>
        /// <returns>`ResponseDto` object.</returns>
        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeader = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);

                orderHeader.OrderTime = DateTime.Now;
                orderHeader.Status = SD.STATUS_PENDING;
                orderHeader.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);
                orderHeader.OrderTotal = Math.Round(orderHeader.OrderTotal, 2);

                OrderHeader orderCreated = _orderDbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeader)).Entity;

                await _orderDbContext.SaveChangesAsync();

                orderHeader.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeader;
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Creates a Stripe session for payment processing, including line
        /// items and discounts, and updates the order header with the session ID.</summary>
        /// <param name="stripeRequestDto">Contains the necessary information for creating
        /// a Stripe session.</param>
        /// <returns>ResponseDto object.</returns>
        [Authorize]
        [HttpPost("CreateStripeSession")]
        public ResponseDto CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                var sessionOptions = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",

                };

                var DiscountsObj = new List<SessionDiscountOptions>()
                {
                    new() { Coupon = stripeRequestDto.OrderHeader!.CouponCode }
                };

                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails!)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.99 -> 2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product!.Name
                            }
                        },
                        Quantity = item.Count
                    };

                    sessionOptions.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDto.OrderHeader.Discount > 0) sessionOptions.Discounts = DiscountsObj;

                var service = new SessionService();

                Session session = service.Create(sessionOptions);

                stripeRequestDto.StripeSessionUrl = session.Url;

                OrderHeader orderHeader = _orderDbContext.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);

                orderHeader.StripeSessionId = session.Id;

                _orderDbContext.SaveChanges();

                _response.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.Success = false;
            }

            return _response;
        }

        /// <summary>Validates a Stripe session and updates the order status if the payment is successful.</summary>
        /// <param name="orderHeaderId">Represents the unique identifier of an order header.</param>
        /// <returns>`ResponseDto` object.</returns>
        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _orderDbContext.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

                var sessionService = new SessionService();

                Session session = sessionService.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();

                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.STATUS_APPROVED;

                    _orderDbContext.SaveChanges();

                    RewardsDto rewards = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;

                    await _messageBus.PublishMessageAsync(rewards, topicName);

                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.Success = false;
            }

            return _response;
        }

        /// <summary>Updates the status of an order in a database and creates a refund
        /// if the new status is "cancelled".</summary>
        /// <param name="orderId">Represents the unique identifier of the order.</param>
        /// <param name="newStatus">Represents the updated status of an order.</param>
        /// <returns>ResponseDto object.</returns>
        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public ResponseDto UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _orderDbContext.OrderHeaders.First(u => u.OrderHeaderId == orderId);

                if (orderHeader != null)
                {
                    if (newStatus == SD.STATUS_CANCELLED)
                    {
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId
                        };

                        var service = new RefundService();

                        Refund refund = service.Create(options);
                    }

                    orderHeader.Status = newStatus;

                    _orderDbContext.SaveChanges();
                }
            }
            catch (Exception) { _response.Success = false; }

            return _response;
        }
    }
}