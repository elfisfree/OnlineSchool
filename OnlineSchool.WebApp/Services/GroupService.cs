using OnlineSchool.Shared.DTOs;
using System.Net.Http.Json;

namespace OnlineSchool.WebApp.Services
{
    public class GroupService : IGroupService
    {
        private readonly HttpClient _httpClient;

        public GroupService(IHttpClientFactory clientFactory, IAuthService authService)
        {
            _httpClient = clientFactory.CreateClient("OnlineSchool.API");
            authService.OnTokenChanged += SetAuthToken;
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token) ? null : new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<GroupDto>> GetGroupsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<GroupDto>>("api/groups");
        }

        public async Task CreateGroupAsync(CreateGroupDto group)
        {
            await _httpClient.PostAsJsonAsync("api/groups", group);
        }
    }
}