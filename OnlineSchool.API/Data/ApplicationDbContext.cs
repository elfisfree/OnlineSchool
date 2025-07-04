using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.Shared.Models;

namespace OnlineSchool.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<EducationalProgram> EducationalPrograms { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<ScheduledLesson> ScheduledLessons { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<StudentProgress> StudentProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Настраиваем связь многие-ко-многим между Группой и Учениками (ApplicationUser)
            builder.Entity<Group>()
                .HasMany(g => g.Students)
                .WithMany(u => u.StudentGroups);

            // Настраиваем связь один-ко-многим между Преподавателем (ApplicationUser) и Группой
            builder.Entity<Group>()
                .HasOne(g => g.Teacher)
                .WithMany(u => u.TaughtGroups)
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict); // Запрещаем удаление преподавателя, если у него есть группы
            
            builder.Entity<ScheduledLesson>()
            .HasOne(sl => sl.Lesson)
            .WithMany() // У Lesson нет прямого навигационного свойства на ScheduledLesson, поэтому WithMany() пустой
            .HasForeignKey(sl => sl.LessonId)
            .OnDelete(DeleteBehavior.Restrict); // Заменяем CASCADE на RESTRICT (или NO ACTION)


        }

    }
}