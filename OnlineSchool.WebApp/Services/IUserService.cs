using OnlineSchool.Shared.DTOs;

namespace OnlineSchool.WebApp.Services
{
    public interface IUserService
    {
        void SetAuthToken(string token);
        Task<List<UserDto>> GetUsersByRoleAsync(string roleName);
    }
}