using OnlineSchool.Shared.DTOs;

public interface IStudentService
{
    void SetAuthToken(string token);
    Task<List<ScheduledLessonDto>> GetMyScheduleAsync();
    Task<LessonDto> GetLessonDetailsAsync(int lessonId);
}