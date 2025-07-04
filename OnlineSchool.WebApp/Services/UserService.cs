using OnlineSchool.Shared.DTOs;
using System.Net.Http.Json;

namespace OnlineSchool.WebApp.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(IHttpClientFactory clientFactory, IAuthService authService)
        {
            _httpClient = clientFactory.CreateClient("OnlineSchool.API");
            authService.OnTokenChanged += SetAuthToken;
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token) ? null : new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<UserDto>> GetUsersByRoleAsync(string roleName)
        {
            return await _httpClient.GetFromJsonAsync<List<UserDto>>($"api/users/role/{roleName}");
        }
    }
}