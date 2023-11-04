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

        /// <summary>Assigns a role to a user with a given email address.</summary>
        /// <param name="email">Represents the email address of the user you want to assign a role to.</param>
        /// <param name="roleName">Represents the name of the role that you want to
        /// assign to the user.</param>
        /// <returns>The method is returning a boolean value. It returns true if the role
        /// is successfully assigned to the user, and false if the user is not found in
        /// the database.</returns>
        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _authDbContext.AppUsers.FirstOrDefault(u => u.Email!.ToLower() == email.ToLower());

            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }

                await _userManager.AddToRoleAsync(user, roleName);

                return true;
            }

            return false;
        }

        /// <summary>Takes a login request, checks if the user exists and the password is
        /// valid, generates a JWT token, and returns a login response with the
        /// user information and token.</summary>
        /// <param name="LoginRequestDto">Contains the following properties:</param>
        /// <returns>`Task<LoginResponseDto>`.</returns>
        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _authDbContext.AppUsers.FirstOrDefault(u => u.UserName!.ToLower() == loginRequestDto.UserName!.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user!, loginRequestDto.Password!);

            if (user == null || !isValid) return new LoginResponseDto { User = null, Token = "" };

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDto = new()
            {
                ID = user.Id,
                Name = user.Name,
                Phone = user.PhoneNumber,
                Email = user.Email
            };

            return new LoginResponseDto { User = userDto, Token = token };
        }

        /// <summary>Takes a `RegisterRequestDto` as input and create a new
        /// user</summary>
        /// <param name="RegisterRequestDto">Contains the registration information for
        /// a user.</param>
        /// <returns>String message.</returns>
        public async Task<string> Register(RegisterRequestDto registerRequestDto)
        {
            AppUser user = new()
            {
                Name = registerRequestDto.Name,
                Email = registerRequestDto.Email,
                PhoneNumber = registerRequestDto.Phone,
                UserName = registerRequestDto.Email,
                NormalizedEmail = registerRequestDto.Email!.ToUpper()
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerRequestDto.Password!);

                if (result.Succeeded)
                {
                    var newUser = _authDbContext.AppUsers.First(u => u.UserName == registerRequestDto.Email);

                    UserDto userDto = new()
                    {
                        ID = newUser.Id,
                        Name = newUser.Name,
                        Phone = newUser.PhoneNumber,
                        Email = newUser.Email
                    };

                    return "";
                }

                return result.Errors.FirstOrDefault()!.Description;
            }
            catch (Exception) { }

            return "Error encontrado";
        }
    }
}