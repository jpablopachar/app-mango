using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using client.Interfaces;
using client.Models;
using client.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace client.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        /// <summary>Returns a view for the login page.</summary>
        /// <returns>View with the loginRequestDto as the model.</returns>
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();

            return View(loginRequestDto);
        }

        /// <summary>Handles the login process by sending a login request, validating
        /// the response, and redirecting the user to the home page if successful.</summary>
        /// <param name="LoginRequestDto">Contains the information for a user to
        /// log in.</param>
        /// <returns>IActionResult. If the responseDto is not null and has a success
        /// value of true, the method will redirect to the "Index" action of the "Home"
        /// controller. Otherwise, it will return the current view with the loginRequestDto
        /// model, and set the "error" value in TempData.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);

            if (responseDto != null && responseDto.Success)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result)!)!;

                await SignInUser(loginResponseDto);

                _tokenProvider.SetToken(loginResponseDto.Token!);

                return RedirectToAction("Index", "Home");
            }

            TempData["error"] = responseDto!.Message;

            return View(loginRequestDto);
        }

        /// <summary>Returns a view for the registration page and passes a list of role options to the view.</summary>
        /// <returns>View result.</returns>
        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>
            {
                new() { Text = SD.ROLE_ADMIN, Value = SD.ROLE_ADMIN },
                new() { Text = SD.ROLE_CUSTOMER, Value = SD.ROLE_CUSTOMER }
            };

            ViewBag.RoleList = roleList;

            return View();
        }

        /// <summary>Which also assigns a role to the registered user and redirects to
        /// the login page if successful.</summary>
        /// <param name="RegisterRequestDto">Contains the information to register a
        /// user.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            ResponseDto responseDto = await _authService.RegisterAsync(registerRequestDto);

            ResponseDto assignRole;

            if (responseDto != null && responseDto.Success)
            {
                if (string.IsNullOrEmpty(registerRequestDto.Role)) registerRequestDto.Role = SD.ROLE_CUSTOMER;

                assignRole = await _authService.AssignRoleAsync(registerRequestDto);

                if (assignRole != null && assignRole.Success)
                {
                    TempData["success"] = "Registro satisfactorio";

                    return RedirectToAction(nameof(Login));
                }
            }

            TempData["error"] = responseDto!.Message;

            var roleList = new List<SelectListItem>
            {
                new() { Text = SD.ROLE_ADMIN, Value = SD.ROLE_ADMIN },
                new() { Text = SD.ROLE_CUSTOMER, Value = SD.ROLE_CUSTOMER }
            };

            ViewBag.RoleList = roleList;

            return View(registerRequestDto);
        }

        /// <summary>Signing them out and clearing their token, then redirects them
        /// to the home page.</summary>
        /// <returns>IActionResult, which is typically used to represent the result of
        /// an action method in ASP.NET Core. In this case, the method is redirecting to
        /// the "Index" action method of the "Home" controller.</returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            _tokenProvider.ClearToken();

            return RedirectToAction("Index", "Home");
        }

        /// <summary>Extracts the necessary claims from the JWT token, creates a
        /// ClaimsIdentity and ClaimsPrincipal, and signs in the user using Cookie
        /// Authentication.</summary>
        /// <param name="LoginResponseDto">Contains the response data from a login
        /// request.</param>
        private async Task SignInUser(LoginResponseDto loginResponseDto)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(loginResponseDto.Token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email)!.Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)!.Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name)!.Value));

            identity.AddClaim(new Claim(ClaimTypes.Email, token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email)!.Value));

            identity.AddClaim(new Claim(ClaimTypes.Role, token.Claims.FirstOrDefault(u => u.Type == "role")!.Value));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}