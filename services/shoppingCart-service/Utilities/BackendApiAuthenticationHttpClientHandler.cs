using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace shoppingCart_service.Utilities
{
    public class BackendApiAuthenticationHttpClientHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>Adds an access token to the Authorization header of an HTTP request.
        /// </summary>
        /// <param name="HttpRequestMessage">Represents an HTTP request message
        /// that is sent from a client to a server.</param>
        /// <param name="CancellationToken">Is a token that can be used to request
        /// cancellation of an asynchronous operation.</param>
        /// <returns>The asynchronous operation of sending an HTTP response message.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}