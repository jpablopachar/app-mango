using Newtonsoft.Json;
using shoppingCart_service.Dtos;
using shoppingCart_service.Interfaces;

namespace shoppingCart_service.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>Retrieves a coupon from an API using a coupon code and returns it
        /// as a `CouponDto` object.</summary>
        /// <param name="couponCode">Represents the code of the coupon that you want to
        /// retrieve.</param>
        /// <returns>Task of type CouponDto.</returns>
        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if (resp != null && resp.Success) return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result)!)!;

            return new CouponDto();
        }
    }
}