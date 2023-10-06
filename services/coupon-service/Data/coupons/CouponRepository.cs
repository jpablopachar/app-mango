using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coupon_service.Models;
using Microsoft.EntityFrameworkCore;

namespace coupon_service.Data.coupons
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _context;

        public CouponRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateCoupon(Coupon coupon)
        {
            if (coupon is null)
            {
                throw new BadHttpRequestException("Los datos del cupón son incorrectos", 400);
            }

            await _context.Coupons.AddAsync(coupon);

            return await _context.SaveChangesAsync();
        }

        public Task DeleteCoupon(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Coupon> GetCouponById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Coupon>> GetCoupons()
        {
            return await _context.Coupons.ToListAsync();
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