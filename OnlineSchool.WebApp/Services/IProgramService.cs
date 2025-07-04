using OnlineSchool.Shared.DTOs;

namespace OnlineSchool.WebApp.Services
{
    public interface IProgramService
    {
        void SetAuthToken(string token);
        Task<List<ProgramDto>> GetProgramsAsync();
        Task<ProgramDto> GetProgramByIdAsync(int id);
        Task CreateProgramAsync(ProgramDto program);
        Task UpdateProgramAsync(ProgramDto program);
        Task DeleteProgramAsync(int id);

        Task<List<CourseDto>> GetCoursesAsync(int programId);
        Task CreateCourseAsync(int programId, CourseDto course);
        Task<List<LessonDto>> GetLessonsAsync(int courseId);
        Task CreateLessonAsync(int courseId, LessonDto lesson);

        Task DeleteCourseAsync(int programId, int courseId);
        Task DeleteLessonAsync(int courseId, int lessonId);
    }
}