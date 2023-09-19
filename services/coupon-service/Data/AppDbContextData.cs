using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace coupon_service.Data
{
    public class AppDbContextData
    {
        public static async Task LoadDataAsync(AppDbContext context, ILoggerFactory loggerFactory) {
            try {
                if (!context.Coupons.Any()) {
                    await context.SaveChangesAsync();
                }
            } catch (Exception exception) {
                var logger = loggerFactory.CreateLogger<AppDbContextData>();

                logger.LogError(exception.Message);
            }
        }
    }
}