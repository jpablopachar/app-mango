using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using coupon_service.Data.coupons;
using Microsoft.AspNetCore.Mvc;

namespace coupon_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        private IMapper _mapper;
    }

    [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetProperties()
        {
            var properties = await _propertyRepository.GetAllProperties();

            return Ok(_mapper.Map<IEnumerable<PropertyResponseDto>>(properties));
        }
}