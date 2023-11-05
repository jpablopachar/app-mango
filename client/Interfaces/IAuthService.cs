using client.Models;

namespace client.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto);

        Task<ResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto);

        Task<ResponseDto> AssignRoleAsync(RegisterRequestDto registerRequestDto);
    }
}