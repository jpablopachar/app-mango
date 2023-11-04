using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using auth_service.Dtos;

namespace auth_service.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterRequestDto registerRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}