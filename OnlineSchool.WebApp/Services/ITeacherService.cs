using OnlineSchool.Shared.DTOs;

namespace OnlineSchool.WebApp.Services
{
    public interface ITeacherService
    {
        Task<List<TeacherGroupDto>> GetMyGroupsAsync();
        Task<GroupDetailDto> GetGroupDetailsAsync(int groupId);
        Task<List<ScheduledLessonDto>> GetGroupScheduleAsync(int groupId);
        Task<LessonDto> GetLessonDetailsAsync(int lessonId);
        Task UpdateAttendanceAsync(int scheduledLessonId, AttendanceUpdateDto dto);
    }
}