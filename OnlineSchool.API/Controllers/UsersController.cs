using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.Shared.DTOs;
using OnlineSchool.Shared.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: api/users/role/Teacher
    [HttpGet("role/{roleName}")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(string roleName)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
        var usersDto = usersInRole.Select(u => new UserDto
        {
            Id = u.Id,
            FullName = $"{u.FirstName} {u.LastName}",
            Email = u.Email
        }).ToList();

        return Ok(usersDto);
    }
}