using reward_service.Message;

namespace reward_service.Interfaces
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}