using client.Interfaces;
using client.Models;
using client.Utilities;

namespace client.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        /// <summary>Takes a RegisterRequestDto as input and sends a POST request to
        /// the specified URL with the registerRequestDto as the data.</summary>
        /// <param name="RegisterRequestDto">Contains the information for registering
        /// a user.
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> AssignRoleAsync(RegisterRequestDto registerRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = registerRequestDto,
                Url = $"{SD.AuthApi}/api/auth/assignRole"
            });
        }

        /// <summary>Sends a POST request to the authentication API with a login
        /// request</summary>
        /// <param name="LoginRequestDto">Contains the information for a login
        /// request.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = $"{SD.AuthApi}/api/auth/login"
            }, withBearer: false);
        }

        /// <summary>ends a POST request to the specified URL with register request
        /// data and returns a response.</summary>
        /// <param name="RegisterRequestDto">Contains the necessary information
        /// for registering a user.</param>
        /// <returns>Returning a Task of type ResponseDto.</returns>
        public async Task<ResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = registerRequestDto,
                Url = $"{SD.AuthApi}/api/auth/register"
            }, withBearer: false);
        }
    }
}