using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.Shared.Models
{
    public class EducationalProgram
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}