using OnlineSchool.Shared.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OnlineSchool.WebApp.Services
{
    public class ProgramService : IProgramService
    {
        private readonly HttpClient _httpClient;

        public void SetAuthToken(string token) // <-- РЕАЛИЗУЙТЕ МЕТОД
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

        public ProgramService(IHttpClientFactory clientFactory, IAuthService authService)
        {
            _httpClient = clientFactory.CreateClient("OnlineSchool.API");

            authService.OnTokenChanged += SetAuthToken;
        }

        public async Task<List<ProgramDto>> GetProgramsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProgramDto>>("api/programs");
        }

        public async Task<ProgramDto> GetProgramByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ProgramDto>($"api/programs/{id}");
        }

        public async Task CreateProgramAsync(ProgramDto program)
        {
            await _httpClient.PostAsJsonAsync("api/programs", program);
        }

        public async Task UpdateProgramAsync(ProgramDto program)
        {
            await _httpClient.PutAsJsonAsync($"api/programs/{program.Id}", program);
        }

        public async Task DeleteProgramAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/programs/{id}");
        }

        public async Task<List<CourseDto>> GetCoursesAsync(int programId)
        {
            return await _httpClient.GetFromJsonAsync<List<CourseDto>>($"api/programs/{programId}/courses");
        }

        public async Task CreateCourseAsync(int programId, CourseDto course)
        {
            await _httpClient.PostAsJsonAsync($"api/programs/{programId}/courses", course);
        }

        public async Task<List<LessonDto>> GetLessonsAsync(int courseId)
        {
            return await _httpClient.GetFromJsonAsync<List<LessonDto>>($"api/courses/{courseId}/lessons");
        }

        public async Task CreateLessonAsync(int courseId, LessonDto lesson)
        {
            await _httpClient.PostAsJsonAsync($"api/courses/{courseId}/lessons", lesson);
        }

        public async Task DeleteCourseAsync(int programId, int courseId)
        {
            await _httpClient.DeleteAsync($"api/programs/{programId}/courses/{courseId}");
        }

        public async Task DeleteLessonAsync(int courseId, int lessonId)
        {
            await _httpClient.DeleteAsync($"api/courses/{courseId}/lessons/{lessonId}");
        }
    }
}