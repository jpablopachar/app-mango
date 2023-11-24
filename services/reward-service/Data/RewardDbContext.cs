using Microsoft.EntityFrameworkCore;
using reward_service.Models;

namespace reward_service.Data
{
    public class RewardDbContext : DbContext
    {
        public RewardDbContext(DbContextOptions<RewardDbContext> options) : base(options)
        { }

        public DbSet<Reward> Rewards { get; set; }
    }
}