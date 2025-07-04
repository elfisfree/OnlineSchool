using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.Shared.DTOs
{
    public class ScheduleGenerationDto
    {
        [Required]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        public TimeSpan StartTime { get; set; } = new TimeSpan(19, 0, 0); // 19:00

        public List<DayOfWeek> DaysOfWeek { get; set; } = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday };
    }
}