namespace reward_service.Message
{
    public class RewardsMessage
    {
        public string? UserId { get; set; }
        public int RewardsActivity { get; set; }
        public int OrderId { get; set; }
    }
}