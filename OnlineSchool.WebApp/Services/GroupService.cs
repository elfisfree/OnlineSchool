using OnlineSchool.Shared.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OnlineSchool.WebApp.Services
{
    public class GroupService : IGroupService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenStorageService _tokenStorage;

        public GroupService(IHttpClientFactory clientFactory, TokenStorageService tokenStorage)
        {
            _httpClient = clientFactory.CreateClient("OnlineSchool.API");
            _tokenStorage = tokenStorage;
        }

        public void SetAuthToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<GroupDto>> GetGroupsAsync()
        {
            // Здесь мы НЕ вызываем SetAuthToken. Мы доверяем тому, что его вызвали извне.
            return await _httpClient.GetFromJsonAsync<List<GroupDto>>("api/groups");
        }

        //private void SetAuthorizationHeader()
        //{
        //    var token = _tokenStorage.Token;
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    }
        //}

        //public void SetAuthToken(string token)
        //{
        //    _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token) ? null : new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        //}

        //public async Task<List<GroupDto>> GetGroupsAsync()
        //{
        //    return await _httpClient.GetFromJsonAsync<List<GroupDto>>("api/groups");
        //}

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

        public async Task<List<ScheduledLessonDto>> GetScheduleAsync(int groupId)
        {
            return await _httpClient.GetFromJsonAsync<List<ScheduledLessonDto>>($"api/groups/{groupId}/schedule");
        }

        public async Task GenerateScheduleAsync(int groupId, ScheduleGenerationDto dto)
        {
            await _httpClient.PostAsJsonAsync($"api/groups/{groupId}/schedule/generate", dto);
        }
    }
}