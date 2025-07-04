using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.Shared.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int EducationalProgramId { get; set; }
    }
}