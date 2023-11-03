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

        /// <summary>Sends a POST request to the specified URL with the provided coupon
        /// data and returns a response.</summary>
        /// <param name="CouponDto">Contains the information to create a coupon.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> CreateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon",
                ApiType = SD.ApiType.POST,
                Data = couponDto
            });
        }

        /// <summary>Sends a DELETE request to the specified URL with the given coupon
        /// ID and returns a `ResponseDto` object.</summary>
        /// <param name="id">The id of the coupon that needs to be deleted.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> DeleteCouponAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon/{id}",
                ApiType = SD.ApiType.DELETE
            });
        }

        /// <summary>Sends a request to the Coupon API to retrieve all coupons.
        /// </summary>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon",
                ApiType = SD.ApiType.GET
            });
        }

        /// <summary>Sends a GET request to a coupon API with a specified coupon
        /// code and returns a `ResponseDto` object.</summary>
        /// <param name="couponCode">Represent the unique identifier of a coupon.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon/{couponCode}",
                ApiType = SD.ApiType.GET
            });
        }

        /// <summary>Sends a request to the Coupon API to retrieve a coupon by its ID.</summary>
        /// <param name="id">Unique identifier of the coupon.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> GetCouponByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{SD.CouponApi}/api/coupon/getByCode/{id}",
                ApiType = SD.ApiType.GET
            });
        }

        /// <summary>Sends a PUT request to the Coupon API with the provided `couponDto` data.
        /// </summary>
        /// <param name="CouponDto">The coupon information to be updated.</param>
        /// <returns>Task of type ResponseDto.</returns>
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