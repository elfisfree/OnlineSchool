using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.API.Data;
using OnlineSchool.Shared.DTOs;
using OnlineSchool.Shared.Models;

[Route("api/courses/{courseId}/lessons")]
[ApiController]
[Authorize(Roles = "Admin")]
public class LessonsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LessonsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/courses/1/lessons
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessons(int courseId)
    {
        var lessons = await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order) // Важно сортировать по порядку
            .Select(l => new LessonDto
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                ContentText = l.ContentText,
                VideoUrl = l.VideoUrl,
                Order = l.Order,
                CourseId = l.CourseId
            })
            .ToListAsync();

        return Ok(lessons);
    }

    // POST: api/courses/1/lessons
    [HttpPost]
    public async Task<ActionResult<LessonDto>> CreateLesson(int courseId, [FromBody] LessonDto lessonDto)
    {
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
        {
            return NotFound("Course not found.");
        }

        var lesson = new Lesson
        {
            Title = lessonDto.Title,
            Description = lessonDto.Description,
            ContentText = lessonDto.ContentText,
            VideoUrl = lessonDto.VideoUrl,
            Order = lessonDto.Order,
            CourseId = courseId
        };

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        lessonDto.Id = lesson.Id;
        lessonDto.CourseId = courseId;

        return CreatedAtAction(nameof(GetLessons), new { courseId = courseId }, lessonDto);
    }

    // DELETE: api/courses/1/lessons/3
    [HttpDelete("{lessonId}")]
    public async Task<IActionResult> DeleteLesson(int courseId, int lessonId)
    {
        var lesson = await _context.Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId && l.CourseId == courseId);

        if (lesson == null)
        {
            return NotFound();
        }

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}