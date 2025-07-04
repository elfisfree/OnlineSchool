using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.API.Data;
using OnlineSchool.Shared.DTOs;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Student")] // Доступ только для учеников
public class StudentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StudentController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    // GET: api/student/my-schedule
    [HttpGet("my-schedule")]
    public async Task<ActionResult<IEnumerable<ScheduledLessonDto>>> GetMySchedule()
    {
        var studentId = GetCurrentUserId();

        // Находим все группы, в которых состоит студент
        var studentGroupIds = await _context.Groups
            .Where(g => g.Students.Any(s => s.Id == studentId))
            .Select(g => g.Id)
            .ToListAsync();

        if (!studentGroupIds.Any())
        {
            return Ok(new List<ScheduledLessonDto>()); // Возвращаем пустой список, если студент не в группах
        }

        // Находим все занятия для этих групп
        var schedule = await _context.ScheduledLessons
            .Where(sl => studentGroupIds.Contains(sl.GroupId))
            .Include(sl => sl.Lesson)
            .OrderBy(sl => sl.ScheduledDateTime)
            .Select(sl => new ScheduledLessonDto
            {
                Id = sl.Id,
                LessonTitle = sl.Lesson.Title,
                ScheduledDateTime = sl.ScheduledDateTime,
                // Проверяем, есть ли запись о посещаемости для ЭТОГО студента
                IsCompleted = sl.Attendances.Any(a => a.StudentId == studentId && a.WasPresent)
            })
            .ToListAsync();

        return Ok(schedule);
    }

    // GET: api/student/lesson-details/5
    [HttpGet("lesson-details/{lessonId}")]
    public async Task<ActionResult<LessonDto>> GetLessonDetails(int lessonId)
    {
        var studentId = GetCurrentUserId();

        // --- НАЧАЛО ИСПРАВЛЕННОЙ ЛОГИКИ ---

        // Шаг 1: Находим все ID групп, в которых состоит студент.
        var studentGroupIds = await _context.Groups
            .Where(g => g.Students.Any(s => s.Id == studentId))
            .Select(g => g.Id)
            .ToListAsync();

        if (!studentGroupIds.Any())
        {
            return Forbid();
        }

        // Шаг 2: Проверяем, есть ли запрошенный урок в расписании хотя бы одной из этих групп.
        var hasAccess = await _context.ScheduledLessons
            .AnyAsync(sl => sl.LessonId == lessonId && studentGroupIds.Contains(sl.GroupId));

        if (!hasAccess)
        {
            return Forbid();
        }

        // --- КОНЕЦ ИСПРАВЛЕННОЙ ЛОГИКИ ---

        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null)
        {
            return NotFound();
        }

        var lessonDto = new LessonDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Description = lesson.Description,
            ContentText = lesson.ContentText,
            VideoUrl = lesson.VideoUrl,
            Order = lesson.Order,
            CourseId = lesson.CourseId
        };
        return Ok(lessonDto);
    }
}