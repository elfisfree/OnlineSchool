using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OnlineSchool.Shared.DTOs;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OnlineSchool.WebApp.Services
{
    public class AuthService : IAuthService
    {
        public event Action<string> OnTokenChanged;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(IHttpClientFactory clientFactory,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = clientFactory.CreateClient("OnlineSchool.API");
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public void SetAuthToken(string token) 
        {
            if (string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<AuthResult> Login(LoginDto loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);
            if (!response.IsSuccessStatusCode)
            {
                return new AuthResult { Successful = false, Error = "Invalid username or password." };
            }

            var content = await response.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            await _localStorage.SetItemAsync("authToken", loginResult.Token);

            SetAuthToken(loginResult.Token);

            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).UpdateAuthenticationState(loginResult.Token);

            OnTokenChanged?.Invoke(loginResult.Token);

            return new AuthResult { Successful = true };
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            // Вызываем обновление состояния с пустым токеном
            SetAuthToken(null);

            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).UpdateAuthenticationState(null);
            
            OnTokenChanged?.Invoke(null);
        }


        public async Task<AuthResult> Register(RegisterDto registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);
            if (response.IsSuccessStatusCode)
            {
                return new AuthResult { Successful = true };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new AuthResult { Successful = false, Error = errorContent };
        }
    }

    public interface IAuthService
    {
        event Action<string> OnTokenChanged;
        Task<AuthResult> Register(RegisterDto registerModel);
        Task<AuthResult> Login(LoginDto loginModel);
        Task Logout();
        void SetAuthToken(string token);
    }

    public class AuthResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
    }

    public class LoginResult
    {
        public string Token { get; set; }
    }
}