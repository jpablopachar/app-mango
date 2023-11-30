using auth_service.Dtos;
using auth_service.Interfaces;
using message_bus;
using Microsoft.AspNetCore.Mvc;

namespace auth_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;

        public AuthController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            _authService = authService;
            _messageBus = messageBus;
            _configuration = configuration;
            _response = new();
        }

        /// <summary>Calls an authentication service to register a user and returns an appropriate response.</summary>
        /// <param name="RegisterRequestDto">Contains the necessary information for
        /// registering a user.</param>
        /// <returns>IActionResult. If there is an error message, it returns a
        /// BadRequest with the error message in the response body.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var errorMessage = await _authService.Register(registerRequestDto);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.Success = false;
                _response.Message = errorMessage;

                return BadRequest(_response);
            }

            await _messageBus.PublishMessageAsync(registerRequestDto.Email!, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")!);

            return Ok(_response);
        }

        /// <summary>Call an authentication service to perform the login</summary>
        /// <param name="LoginRequestDto">Contains the necessary information for a user
        /// to login.</param>
        /// <returns>IActionResult. If the loginResponseDto.User is null, it returns a
        /// BadRequest with the _response object as the response body.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponseDto = await _authService.Login(loginRequestDto);

            if (loginResponseDto.User == null)
            {
                _response.Success = false;
                _response.Message = "El username o la contraseña son incorrectos";

                return BadRequest(_response);
            }

            _response.Result = loginResponseDto;

            return Ok(_response);
        }

        /// <summary>Assigns a role to a user based on the provided email and role in
        /// the request body.</summary>
        /// <param name="RegisterRequestDto">Contains the following properties.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterRequestDto registerRequestDto)
        {
            var assignRoleSuccessful = await _authService.AssignRole(registerRequestDto.Email!, registerRequestDto.Role!.ToUpper());

            if (!assignRoleSuccessful)
            {
                _response.Success = false;
                _response.Message = "El usuario no existe";

                return BadRequest(_response);
            }

            return Ok(_response);
        }
    }
}