using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.Shared.DTOs
{
    public class CreateGroupDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int EducationalProgramId { get; set; }
        [Required]
        public string TeacherId { get; set; }
    }
}