using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSchool.Shared.Models
{
    public class StudentProgress
    {
        [Key]
        public int Id { get; set; }
        public bool IsCompleted { get; set; }

        // Связь с учеником
        public string StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual ApplicationUser Student { get; set; }

        // Связь с уроком
        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }
    }
}