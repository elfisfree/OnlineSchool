using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSchool.Shared.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        public string? Description { get; set; }
        public string? ContentText { get; set; } // Текстовый материал
        public string? VideoUrl { get; set; } // Ссылка на видео

        public int Order { get; set; } // Порядковый номер для последовательности

        // Связь с курсом
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}