using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.Shared.DTOs
{
    public class AddStudentDto
    {
        [Required]
        public string StudentId { get; set; }
    }
}