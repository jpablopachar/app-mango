using shoppingCart_service.Dtos;

namespace shoppingCart_service.Interfaces
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}