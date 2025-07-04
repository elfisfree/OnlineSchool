using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSchool.Shared.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Связь с преподавателем
        public string TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public virtual ApplicationUser Teacher { get; set; }

        // Связь с программой
        public int EducationalProgramId { get; set; }
        [ForeignKey("EducationalProgramId")]
        public virtual EducationalProgram EducationalProgram { get; set; }

        // Связь с учениками (многие-ко-многим)
        public virtual ICollection<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();

        public virtual ICollection<ScheduledLesson> ScheduledLessons { get; set; } = new List<ScheduledLesson>();
    }
}