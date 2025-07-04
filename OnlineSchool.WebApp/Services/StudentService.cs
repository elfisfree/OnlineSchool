using OnlineSchool.Shared.DTOs;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OnlineSchool.WebApp.Services
{
    public class StudentService : IStudentService
    {
        private readonly HttpClient _httpClient;

        public StudentService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("OnlineSchool.API");
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

        public async Task<List<ScheduledLessonDto>> GetMyScheduleAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ScheduledLessonDto>>("api/student/my-schedule");
        }

        public async Task<LessonDto> GetLessonDetailsAsync(int lessonId)
        {
            return await _httpClient.GetFromJsonAsync<LessonDto>($"api/student/lesson-details/{lessonId}");
        }
    }
}