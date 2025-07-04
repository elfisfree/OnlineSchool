using OnlineSchool.Shared.DTOs;

namespace OnlineSchool.WebApp.Services
{
    public interface IGroupService
    {
        void SetAuthToken(string token);
        Task<List<GroupDto>> GetGroupsAsync();
        Task CreateGroupAsync(CreateGroupDto group);
    }
}