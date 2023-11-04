using System.Net;
using System.Text;
using client.Interfaces;
using client.Models;
using Newtonsoft.Json;
using static client.Utilities.SD;

namespace client.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        /// <summary>Sends an HTTP request using the HttpClient class and returns the
        /// response as a ResponseDto object.</summary>
        /// <param name="RequestDto">Contains information about the HTTP
        /// request to be sent.</param>
        /// <param name="withBearer">Indicates whether to include the Bearer token in the request headers.</param>
        /// <returns>Task of type ResponseDto.</returns>
        public async Task<ResponseDto> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage httpRequestMessage = new();
                HttpResponseMessage? httpResponseMessage = null;

                var token = _tokenProvider.GetToken();

                // httpRequestMessage.Headers.Add("Accept", requestDto.ContentType == ContentType.MultipartFormData ? "*/*" : withBearer ? $"Bearer {token}" : "application/json");
                httpRequestMessage.Headers.Add("Accept", "application/json");

                httpRequestMessage.RequestUri = new Uri(requestDto.Url!);

                if (requestDto.ContentType == ContentType.MultipartFormData)
                {
                    var content = new MultipartFormDataContent();

                    foreach (var property in requestDto.Data!.GetType().GetProperties())
                    {
                        var value = property.GetValue(requestDto.Data);

                        if (value is FormFile file && file != null)
                        {

                            content.Add(new StreamContent(file.OpenReadStream()), property.Name, file.FileName);
                        }
                        else
                        {
                            content.Add(new StringContent(value == null ? "" : value.ToString()!), property.Name);
                        }
                    }

                    httpRequestMessage.Content = content;
                }
                else
                {
                    if (requestDto.Data != null) httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? responseMessage = null;

                httpRequestMessage.Method = requestDto.ApiType switch
                {
                    ApiType.POST => HttpMethod.Post,
                    ApiType.DELETE => HttpMethod.Delete,
                    ApiType.PUT => HttpMethod.Put,
                    _ => HttpMethod.Get,
                };

                responseMessage = await httpClient.SendAsync(httpRequestMessage);

                switch (responseMessage!.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        return new ResponseDto { Message = "Unauthorized", Success = false };
                    case HttpStatusCode.Forbidden:
                        return new ResponseDto { Message = "Access Denied", Success = false };
                    case HttpStatusCode.NotFound:
                        return new ResponseDto { Message = "Not Found", Success = false };
                    case HttpStatusCode.InternalServerError:
                        return new ResponseDto { Message = "Internal Server Error", Success = false };
                    default:
                        var apiContent = await responseMessage.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

                        return apiResponseDto!;
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto { Message = ex.Message.ToString(), Success = false };
            }
        }
    }
}