// OnlineSchool.Shared/DTOs/ProgramDto.cs
using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.Shared.DTOs
{
    public class ProgramDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название программы обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Описание обязательно")]
        public string Description { get; set; }
    }
}