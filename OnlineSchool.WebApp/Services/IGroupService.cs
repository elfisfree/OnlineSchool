using OnlineSchool.Shared.DTOs;

namespace OnlineSchool.WebApp.Services
{
    public interface IGroupService
    {
        void SetAuthToken(string token);
        Task<List<GroupDto>> GetGroupsAsync();
        Task CreateGroupAsync(CreateGroupDto group);

        Task<GroupDetailDto> GetGroupDetailsAsync(int id);
        Task AddStudentToGroupAsync(int groupId, string studentId);

        Task RemoveStudentFromGroupAsync(int groupId, string studentId);

        Task<List<ScheduledLessonDto>> GetScheduleAsync(int groupId);
        Task GenerateScheduleAsync(int groupId, ScheduleGenerationDto dto);
    }
}