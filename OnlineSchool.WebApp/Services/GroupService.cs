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


        public async Task<GroupDetailDto> GetGroupDetailsAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<GroupDetailDto>($"api/groups/{id}");
        }

        public async Task AddStudentToGroupAsync(int groupId, string studentId)
        {
            var dto = new AddStudentDto { StudentId = studentId };
            await _httpClient.PostAsJsonAsync($"api/groups/{groupId}/add-student", dto);
        }

        public async Task RemoveStudentFromGroupAsync(int groupId, string studentId)
        {
            await _httpClient.DeleteAsync($"api/groups/{groupId}/remove-student/{studentId}");
        }
    }
}