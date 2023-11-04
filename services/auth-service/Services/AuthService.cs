using auth_service.Data;
using auth_service.Dtos;
using auth_service.Interfaces;
using auth_service.Models;
using Microsoft.AspNetCore.Identity;

namespace auth_service.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _authDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AuthDbContext authDbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _authDbContext = authDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public Task<bool> AssignRole(string email, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<string> Register(RegisterRequestDto registerRequestDto)
        {
            throw new NotImplementedException();
        }
    }
}