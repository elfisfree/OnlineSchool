using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.API.Data;
using OnlineSchool.Shared.DTOs;
using OnlineSchool.Shared.Models;

[Route("api/groups/{groupId}/schedule")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ScheduleController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ScheduleController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/groups/5/schedule
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduledLessonDto>>> GetSchedule(int groupId)
    {
        var schedule = await _context.ScheduledLessons
            .Where(sl => sl.GroupId == groupId)
            .Include(sl => sl.Lesson)
            .OrderBy(sl => sl.ScheduledDateTime)
            .Select(sl => new ScheduledLessonDto
            {
                Id = sl.Id,
                LessonTitle = sl.Lesson.Title,
                ScheduledDateTime = sl.ScheduledDateTime,
                TeacherComment = sl.TeacherComment,
                IsCompleted = false // Заглушка, позже доработаем
            })
            .ToListAsync();

        return Ok(schedule);
    }

    // POST: api/groups/5/schedule/generate
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateSchedule(int groupId, [FromBody] ScheduleGenerationDto dto)
    {
        if (User?.Identity?.IsAuthenticated != true)
        {
            return Unauthorized();
        }

        var group = await _context.Groups
            .Include(g => g.EducationalProgram)
                .ThenInclude(p => p.Courses)
                .ThenInclude(c => c.Lessons)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
        {
            return NotFound("Group not found.");
        }

        // Удаляем старое расписание, если оно было
        var existingSchedule = _context.ScheduledLessons.Where(sl => sl.GroupId == groupId);
        _context.ScheduledLessons.RemoveRange(existingSchedule);

        // Собираем все уроки программы в один список, отсортированный по порядку
        var allLessons = group.EducationalProgram.Courses
            .SelectMany(c => c.Lessons)
            .OrderBy(l => l.Course.Id) // Сначала по порядку курсов
            .ThenBy(l => l.Order)      // Затем по порядку уроков в курсе
            .ToList();

        var scheduledLessons = new List<ScheduledLesson>();
        var currentDate = dto.StartDate;

        foreach (var lesson in allLessons)
        {
            // Ищем следующий подходящий день недели
            while (!dto.DaysOfWeek.Contains(currentDate.DayOfWeek))
            {
                currentDate = currentDate.AddDays(1);
            }

            var lessonDateTime = currentDate.Date + dto.StartTime;
            scheduledLessons.Add(new ScheduledLesson
            {
                GroupId = groupId,
                LessonId = lesson.Id,
                ScheduledDateTime = lessonDateTime
            });

            // Переходим к следующему дню для поиска
            currentDate = currentDate.AddDays(1);
        }

        await _context.ScheduledLessons.AddRangeAsync(scheduledLessons);
        await _context.SaveChangesAsync();

        return Ok("Schedule generated successfully.");
    }
}