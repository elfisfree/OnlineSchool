using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.API.Data;
using OnlineSchool.Shared.DTOs;
using OnlineSchool.Shared.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class GroupsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GroupsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/groups
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups()
    {
        var groups = await _context.Groups
            .Include(g => g.EducationalProgram)
            .Include(g => g.Teacher)
            .Include(g => g.Students)
            .Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                ProgramName = g.EducationalProgram.Name,
                TeacherName = g.Teacher.FirstName + " " + g.Teacher.LastName,
                StudentsCount = g.Students.Count
            })
            .ToListAsync();

        return Ok(groups);
    }

    // POST: api/groups
    [HttpPost]
    public async Task<ActionResult<Group>> CreateGroup([FromBody] CreateGroupDto createGroupDto)
    {
        var group = new Group
        {
            Name = createGroupDto.Name,
            EducationalProgramId = createGroupDto.EducationalProgramId,
            TeacherId = createGroupDto.TeacherId
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGroups), new { id = group.Id }, group);
    }
}