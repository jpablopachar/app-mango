using Microsoft.EntityFrameworkCore;

namespace coupon_service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Models.Coupon> Coupons { get; set; }
    }
}