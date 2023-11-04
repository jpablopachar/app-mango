using AutoMapper;
using coupon_service.Data;
using coupon_service.Dtos;
using coupon_service.Models;
using Microsoft.AspNetCore.Mvc;

namespace coupon_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;

        public CouponController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _response = new();
            _mapper = mapper;
        }

        /// <summary>Retrieves a list of coupons from the database and maps them
        /// to a DTO, returning the result in a ResponseDto.</summary>
        /// <returns>ResponseDto object.</returns>
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> coupons = _context.Coupons.ToList();

                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Retrieves a coupon by its ID and returns it as a response.</summary>
        /// <param name="id">Represents the unique identifier of a coupon.</param>
        /// <returns>ResponseDto object.</returns>
        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon coupon = _context.Coupons.First(c => c.CouponId == id);

                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Retrieves a coupon by its code and returns a response containing
        /// the coupon information.</summary>
        /// <param name="code">Represents the coupon code.</param>
        /// <returns>ResponseDto object.</returns>
        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon coupon = _context.Coupons.First(c => c.CouponCode!.ToLower() == code.ToLower());

                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Receives a CouponDto object, maps it to a Coupon object, adds it to the
        /// database, and returns a ResponseDto object.</summary>
        /// <param name="CouponDto">Represents the coupon information.</param>
        /// <returns>ResponseDto object.</returns>
        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);

                _context.Coupons.Add(coupon);
                _context.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Updates a coupon in the database and returns a response.</summary>
        /// <param name="CouponDto">Represents the coupon information.</param>
        /// <returns>ResponseDto object.</returns>
        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);

                _context.Coupons.Update(coupon);
                _context.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>Deletes a coupon from the database based on its ID.</summary>
        /// <param name="id">Represents the unique identifier of the coupon that needs
        /// to be deleted.</param>
        /// <returns>ResponseDto object.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon coupon = _context.Coupons.First(c => c.CouponId == id);

                _context.Coupons.Remove(coupon);
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