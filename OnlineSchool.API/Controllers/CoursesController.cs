using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.API.Data;
using OnlineSchool.Shared.DTOs;
using OnlineSchool.Shared.Models;

[Route("api/programs/{programId}/courses")]
[ApiController]
[Authorize(Roles = "Admin")]
public class CoursesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CoursesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/programs/1/courses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses(int programId)
    {
        var courses = await _context.Courses
            .Where(c => c.EducationalProgramId == programId)
            .Select(c => new CourseDto { Id = c.Id, Name = c.Name, EducationalProgramId = c.EducationalProgramId })
            .ToListAsync();

        return Ok(courses);
    }

    // POST: api/programs/1/courses
    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse(int programId, [FromBody] CourseDto courseDto)
    {
        var programExists = await _context.EducationalPrograms.AnyAsync(p => p.Id == programId);
        if (!programExists)
        {
            return NotFound("Program not found.");
        }

        var course = new Course
        {
            Name = courseDto.Name,
            EducationalProgramId = programId
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        courseDto.Id = course.Id;
        courseDto.EducationalProgramId = programId;

        return CreatedAtAction(nameof(GetCourses), new { programId = programId }, courseDto);
    }

    // DELETE: api/programs/1/courses/2
    [HttpDelete("{courseId}")]
    public async Task<IActionResult> DeleteCourse(int programId, int courseId)
    {
        // Находим курс, убедившись, что он принадлежит нужной программе
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId && c.EducationalProgramId == programId);

        if (course == null)
        {
            return NotFound();
        }

        // EF Core автоматически удалит связанные уроки, так как у нас настроен каскад
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Мы пока не будем реализовывать Update и Delete для курсов, чтобы не усложнять.
    // Основной фокус на создании.
}