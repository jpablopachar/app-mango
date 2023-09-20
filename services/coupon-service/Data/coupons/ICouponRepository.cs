using coupon_service.Models;

namespace coupon_service.Data.coupons
{
    public interface ICouponRepository
    {
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<Coupon>> GetCoupons();
        Task<Coupon> GetCouponById(int id);
        Task CreateCoupon(Coupon coupon);
        Task UpdateCoupon(Coupon coupon);
        Task DeleteCoupon(int id);
    }
}