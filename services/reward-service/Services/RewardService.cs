using Microsoft.EntityFrameworkCore;
using reward_service.Data;
using reward_service.Interfaces;
using reward_service.Message;
using reward_service.Models;

namespace reward_service.Services
{
    public class RewardService : IRewardService
    {
        private readonly DbContextOptions<RewardDbContext> _rewardDbContext;

        public RewardService(DbContextOptions<RewardDbContext> rewardDbContext)
        {
            _rewardDbContext = rewardDbContext;
        }

        /// <summary>Updates the rewards information in the database based on the
        /// provided `RewardsMessage` object.</summary>
        /// <param name="rewardsMessage">Contains the information needed to update rewards.</param>
        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Reward rewards = new()
                {
                    OrderId = rewardsMessage.OrderId,
                    RewardsActivity = rewardsMessage.RewardsActivity,
                    UserId = rewardsMessage.UserId,
                    RewardsDate = DateTime.Now
                };

                await using var _db = new RewardDbContext(_rewardDbContext);

                await _db.Rewards.AddAsync(rewards);

                await _db.SaveChangesAsync();
            }
            catch (Exception) { }
        }
    }
}