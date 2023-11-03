using client.Models;

namespace client.Interfaces
{
    public interface IBaseService
    {
        /// <summary>Sends an asynchronous request with an optional bearer token.</summary>
        /// <param name="RequestDto">Contains the request data to be sent.</param>
        /// <param name="withBearer">Indicates whether to include a bearer token in the
        /// request headers.</param>
        Task<ResponseDto> SendAsync(RequestDto requestDto, bool withBearer = true);
    }
}