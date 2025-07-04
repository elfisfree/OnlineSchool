namespace OnlineSchool.Shared.DTOs
{
    public class ScheduledLessonDto
    {
        public int Id { get; set; }
        public string LessonTitle { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string TeacherComment { get; set; }
        public bool IsCompleted { get; set; } // Позже будем использовать для отметки преподавателем
    }
}