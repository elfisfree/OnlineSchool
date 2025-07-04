using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.Shared.DTOs
{
    public class LessonDto
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? ContentText { get; set; }
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
        public int CourseId { get; set; }
    }
}