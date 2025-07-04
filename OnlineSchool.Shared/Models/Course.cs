using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSchool.Shared.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Связь с программой
        public int EducationalProgramId { get; set; }
        [ForeignKey("EducationalProgramId")]
        public virtual EducationalProgram EducationalProgram { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}