using OnlineSchool.Shared.DTOs;
using System.Net.Http.Json;

namespace OnlineSchool.WebApp.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenStorageService _tokenStorage; // Используем наш надежный сервис

        public TeacherService(IHttpClientFactory clientFactory, TokenStorageService tokenStorage)
        {
            _httpClient = clientFactory.CreateClient("OnlineSchool.API");
            _tokenStorage = tokenStorage;
        }

        private void SetAuthorizationHeader()
        {
            var token = _tokenStorage.Token;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<TeacherGroupDto>> GetMyGroupsAsync()
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<TeacherGroupDto>>("api/teacher/my-groups");
        }

        public async Task<GroupDetailDto> GetGroupDetailsAsync(int groupId)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<GroupDetailDto>($"api/teacher/group-details/{groupId}");
        }

        public async Task<List<ScheduledLessonDto>> GetGroupScheduleAsync(int groupId)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<ScheduledLessonDto>>($"api/teacher/group-schedule/{groupId}");
        }

        public async Task<LessonDto> GetLessonDetailsAsync(int lessonId)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<LessonDto>($"api/teacher/lesson-details/{lessonId}");
        }

        public async Task UpdateAttendanceAsync(int scheduledLessonId, AttendanceUpdateDto dto)
        {
            SetAuthorizationHeader();
            await _httpClient.PostAsJsonAsync($"api/teacher/scheduled-lesson/{scheduledLessonId}/attendance", dto);
        }
    }
}