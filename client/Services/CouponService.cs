using client.Interfaces;
using client.Models;
using client.Utilities;

namespace client.Services
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;

        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        /// <summary>Creates a new coupon.</summary>
        /// <param name="couponDto">The coupon data transfer object.</param>
        /// <returns>A response data transfer object.</returns>
        public async Task<ResponseDto> CreateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon",
                ApiType = SD.ApiType.POST,
                Data = couponDto
            });
        }

        /// <summary>Deletes a coupon by its ID.</summary>
        /// <param name="id">The ID of the coupon to delete.</param>
        /// <returns>A response data transfer object.</returns>
        public async Task<ResponseDto> DeleteCouponAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon/{id}",
                ApiType = SD.ApiType.DELETE
            });
        }

        /// <summary>Retrieves all coupons from the Coupon API.</summary>
        /// <returns>A response data transfer object.</returns>
        public async Task<ResponseDto> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon",
                ApiType = SD.ApiType.GET
            });
        }

        /// <summary>Retrieves a coupon from the API by its code.</summary>
        /// <param name="couponCode">The code of the coupon to retrieve.</param>
        /// <returns>A response data transfer object.</returns>
        public async Task<ResponseDto> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon/{couponCode}",
                ApiType = SD.ApiType.GET
            });
        }

        /// <summary>Retrieves a coupon by its ID from the Coupon API.</summary>
        /// <param name="id">The ID of the coupon to retrieve.</param>
        /// <returns>A response data transfer object.</returns>
        public async Task<ResponseDto> GetCouponByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon/getByCode/{id}",
                ApiType = SD.ApiType.GET
            });
        }

        /// <summary>Updates a coupon.</summary>
        /// <param name="couponDto">The coupon data transfer object.</param>
        /// <returns>A response data transfer object.</returns>
        public async Task<ResponseDto> UpdateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon",
                ApiType = SD.ApiType.PUT,
                Data = couponDto
            });
        }
    }
}