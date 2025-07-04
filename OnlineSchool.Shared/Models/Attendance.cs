using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSchool.Shared.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        public bool WasPresent { get; set; }

        // Связь с занятием в расписании
        public int ScheduledLessonId { get; set; }
        [ForeignKey("ScheduledLessonId")]
        public virtual ScheduledLesson ScheduledLesson { get; set; }

        // Связь с учеником
        public string StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual ApplicationUser Student { get; set; }
    }
}