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


    // GET: api/groups/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GroupDetailDto>> GetGroupDetails(int id)
    {
        var group = await _context.Groups
            .Include(g => g.EducationalProgram)
            .Include(g => g.Teacher)
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            return NotFound();
        }

        var groupDetailDto = new GroupDetailDto
        {
            Id = group.Id,
            Name = group.Name,
            ProgramName = group.EducationalProgram.Name,
            TeacherName = $"{group.Teacher.FirstName} {group.Teacher.LastName}",
            Students = group.Students.Select(s => new UserDto
            {
                Id = s.Id,
                Email = s.Email,
                FullName = $"{s.FirstName} {s.LastName}"
            }).ToList()
        };

        return Ok(groupDetailDto);
    }

    // POST: api/groups/5/add-student
    [HttpPost("{id}/add-student")]
    public async Task<IActionResult> AddStudentToGroup(int id, [FromBody] AddStudentDto addStudentDto)
    {
        var group = await _context.Groups.Include(g => g.Students).FirstOrDefaultAsync(g => g.Id == id);
        if (group == null)
        {
            return NotFound("Group not found.");
        }

        var student = await _context.Users.FindAsync(addStudentDto.StudentId);
        if (student == null)
        {
            return NotFound("Student not found.");
        }

        // Проверяем, что ученик еще не в группе
        if (group.Students.Any(s => s.Id == student.Id))
        {
            return BadRequest("Student is already in this group.");
        }

        group.Students.Add(student);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // DELETE: api/groups/5/remove-student/student-id-guid
    [HttpDelete("{id}/remove-student/{studentId}")]
    public async Task<IActionResult> RemoveStudentFromGroup(int id, string studentId)
    {
        var group = await _context.Groups.Include(g => g.Students).FirstOrDefaultAsync(g => g.Id == id);
        if (group == null)
        {
            return NotFound("Group not found.");
        }

        var student = group.Students.FirstOrDefault(s => s.Id == studentId);
        if (student == null)
        {
            return NotFound("Student not found in this group.");
        }

        group.Students.Remove(student);
        await _context.SaveChangesAsync();

        return Ok();
    }
}