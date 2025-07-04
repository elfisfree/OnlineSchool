using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.API.Data;
using OnlineSchool.Shared.DTOs;
using OnlineSchool.Shared.Models;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Teacher")] // Весь контроллер только для преподавателей
public class TeacherController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TeacherController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    // GET: api/teacher/my-groups
    [HttpGet("my-groups")]
    public async Task<ActionResult<IEnumerable<TeacherGroupDto>>> GetMyGroups()
    {
        var teacherId = GetCurrentUserId();

        if (string.IsNullOrEmpty(teacherId))
        {
            return Unauthorized(); // На случай, если токен не содержит ID
        }

        var groups = await _context.Groups
            .Where(g => g.TeacherId == teacherId)
            .Include(g => g.EducationalProgram)
            .Include(g => g.Students)
            .Select(g => new TeacherGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                ProgramName = g.EducationalProgram.Name,
                StudentsCount = g.Students.Count
            })
            .ToListAsync();

        return Ok(groups);
    }

    [HttpGet("group-details/{groupId}")]
    public async Task<ActionResult<GroupDetailDto>> GetGroupDetails(int groupId)
    {
        var teacherId = GetCurrentUserId();

        // Загружаем группу и проверяем, что она принадлежит текущему преподавателю
        var group = await _context.Groups
            .Include(g => g.EducationalProgram)
            .Include(g => g.Teacher)
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == groupId && g.TeacherId == teacherId);

        if (group == null)
        {
            // Если группа не найдена или не принадлежит преподавателю, возвращаем 403 Forbidden
            return Forbid();
        }

        // Мы можем переиспользовать DTO, который создали для админа
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

    // GET: api/teacher/group-schedule/5
    [HttpGet("group-schedule/{groupId}")]
    public async Task<ActionResult<IEnumerable<ScheduledLessonDto>>> GetGroupSchedule(int groupId)
    {
        var teacherId = GetCurrentUserId();

        // Проверяем, что группа существует и принадлежит преподавателю
        var isMyGroup = await _context.Groups.AnyAsync(g => g.Id == groupId && g.TeacherId == teacherId);
        if (!isMyGroup)
        {
            return Forbid();
        }

        // Переиспользуем логику получения расписания
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
                IsCompleted = sl.Attendances.Any() // Считаем урок проведенным, если есть хоть одна запись о посещаемости
            })
            .ToListAsync();

        return Ok(schedule);
    }

    // GET: api/teacher/lesson-details/15
    [HttpGet("lesson-details/{lessonId}")]
    public async Task<ActionResult<LessonDto>> GetLessonDetails(int lessonId)
    {
        var teacherId = GetCurrentUserId();

        var hasAccess = await _context.ScheduledLessons
            .AnyAsync(sl => sl.LessonId == lessonId && sl.Group.TeacherId == teacherId);

        if (!hasAccess)
        {
            return Forbid(); // 403 Forbidden
        }

        // Если доступ есть, получаем детали урока
        var lesson = await _context.Lessons
            .Where(l => l.Id == lessonId)
            .Select(l => new LessonDto // Переиспользуем DTO из админки
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                ContentText = l.ContentText,
                VideoUrl = l.VideoUrl,
                Order = l.Order,
                CourseId = l.CourseId
            })
            .FirstOrDefaultAsync();

        if (lesson == null)
        {
            return NotFound();
        }

        return Ok(lesson);
    }

    // POST: api/teacher/scheduled-lesson/10/attendance
    [HttpPost("scheduled-lesson/{scheduledLessonId}/attendance")]
    public async Task<IActionResult> UpdateAttendance(int scheduledLessonId, [FromBody] AttendanceUpdateDto dto)
    {
        var teacherId = GetCurrentUserId();

        // Находим занятие и проверяем, что оно принадлежит преподавателю
        var scheduledLesson = await _context.ScheduledLessons
            .Include(sl => sl.Group)
            .Include(sl => sl.Attendances) // Важно подключить существующие записи
            .FirstOrDefaultAsync(sl => sl.Id == scheduledLessonId && sl.Group.TeacherId == teacherId);

        if (scheduledLesson == null)
        {
            return Forbid("Lesson not found or you don't have access to it.");
        }

        // 1. Обновляем комментарий преподавателя к уроку
        scheduledLesson.TeacherComment = dto.TeacherComment;

        // 2. Получаем всех студентов группы, для которой это занятие
        var studentsInGroup = await _context.Groups
            .Where(g => g.Id == scheduledLesson.GroupId)
            .SelectMany(g => g.Students)
            .ToListAsync();

        // 3. Очищаем старые записи о посещаемости для этого конкретного урока
        _context.Attendances.RemoveRange(scheduledLesson.Attendances);
        await _context.SaveChangesAsync(); // Сохраняем удаление

        // 4. Создаем новые записи о посещаемости
        var newAttendances = studentsInGroup.Select(student => new Attendance
        {
            ScheduledLessonId = scheduledLessonId,
            StudentId = student.Id,
            WasPresent = dto.PresentStudentIds.Contains(student.Id) // Сравниваем строки
        }).ToList();

        await _context.Attendances.AddRangeAsync(newAttendances);

        await _context.SaveChangesAsync(); // Сохраняем новые записи

        return Ok("Attendance updated successfully.");
    }
}