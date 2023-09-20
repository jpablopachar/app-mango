using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coupon_service.Models;

namespace coupon_service.Data.coupons
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _context;

        public CouponRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task CreateCoupon(Coupon coupon)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCoupon(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Coupon> GetCouponById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Coupon>> GetCoupons()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateCoupon(Coupon coupon)
        {
            throw new NotImplementedException();
        }
    }
}