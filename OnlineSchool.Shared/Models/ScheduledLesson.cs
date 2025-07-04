using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSchool.Shared.Models
{
    public class ScheduledLesson
    {
        [Key]
        public int Id { get; set; }

        public DateTime ScheduledDateTime { get; set; }
        public string? TeacherComment { get; set; }

        // Связь с уроком из программы
        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }

        // Связь с группой
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        // Посещаемость для этого занятия
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}