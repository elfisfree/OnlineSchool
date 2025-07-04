using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.Shared.DTOs
{
    public class AttendanceUpdateDto
    {
        [Required]
        public List<string> PresentStudentIds { get; set; } = new List<string>(); // Меняем на string
        public string? TeacherComment { get; set; }
    }
}