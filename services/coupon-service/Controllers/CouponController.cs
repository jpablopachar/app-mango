using AutoMapper;
using coupon_service.Data.coupons;
using coupon_service.Dtos;
using coupon_service.Models;
using Microsoft.AspNetCore.Mvc;

namespace coupon_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        private IMapper _mapper;

        public CouponController(ICouponRepository couponRepository, IMapper mapper)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponResponseDto>>> GetCoupons()
        {
            var coupons = await _couponRepository.GetCoupons();

            return Ok(_mapper.Map<IEnumerable<CouponResponseDto>>(coupons));
        }

        [HttpPost]
        public async Task<ActionResult<CouponResponseDto>> CreateCoupon([FromBody] CouponRequestDto couponRequestDto)
        {
            var coupon = _mapper.Map<Coupon>(couponRequestDto);

            var res = await _couponRepository.CreateCoupon(coupon);

            if (res == 0) throw new Exception("No se pudo agregar el producto");

            var couponResponseDto = _mapper.Map<CouponResponseDto>(coupon);

            return Ok(couponResponseDto);
        }

        /* [HttpPost]
        public async Task<ActionResult<PropertyResponseDto>> CreateProperty([FromBody] PropertyRequestDto propertyRequestDto)
        {
            var property = _mapper.Map<Property>(propertyRequestDto);

            await _propertyRepository.CreateProperty(property);
            await _propertyRepository.SaveChanges();

            var propertyResponseDto = _mapper.Map<PropertyResponseDto>(property);

            return CreatedAtRoute(nameof(GetPropertyById), new { propertyResponseDto.Id }, propertyResponseDto);
        } */
    }
}