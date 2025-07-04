using Microsoft.AspNetCore.Identity;

namespace OnlineSchool.API.Services
{
    public static class RoleInitializer
    {
        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Teacher", "Student" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Создаем роль, если она не существует
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}