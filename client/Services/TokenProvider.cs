using client.Interfaces;
using client.Utilities;

namespace client.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>Clears the JWT token from the HTTP response cookies.</summary>
        public void ClearToken()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(SD.JWT_TOKEN);
        }

        /// <summary>Gets the JWT token from the HTTP request cookies.</summary>
        /// <returns>The JWT token or null if it is not present.</returns>
        public string? GetToken()
        {
            string? token = null;
            bool? hasToken = _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.JWT_TOKEN, out token);

            return hasToken == true ? token : null;
        }

        /// <summary>Sets the JWT token in the response cookie.</summary>
        /// <param name="token">The JWT token to set.</param>
        public void SetToken(string token)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(SD.JWT_TOKEN, token);
        }
    }
}