using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineSchool.WebApp.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public ApiAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return new AuthenticationState(_anonymous);
        }

        public async Task UpdateAuthenticationState(string token)
        {
            ClaimsPrincipal claimsPrincipal;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                claimsPrincipal = _anonymous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);
                if (roles != null)
                {
                    if (roles.ToString().Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());
                        foreach (var parsedRole in parsedRoles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                    }
                }
                keyValuePairs.TryGetValue(ClaimTypes.NameIdentifier, out object nameId);
                if (nameId != null) claims.Add(new Claim(ClaimTypes.NameIdentifier, nameId.ToString()));
                keyValuePairs.TryGetValue(ClaimTypes.Email, out object email);
                if (email != null) claims.Add(new Claim(ClaimTypes.Email, email.ToString()));
            }
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}