using email_service.Dtos;
using email_service.Message;

namespace email_service.Interfaces
{
    public interface IEmailService
    {
        Task EmailCartAndLogAsync(CartDto cartDto);
        Task RegisterUserEmailAndLogAsync(string email);
        Task LogOrderPlacedAsync(RewardsMessage rewardsMessage);
    }
}