using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace OnlineSchool.Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Здесь можно добавить доп. поля, например, имя и фамилию
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Навигационные свойства
        public virtual ICollection<Group> TaughtGroups { get; set; } = new List<Group>(); // Группы, где он преподаватель
        public virtual ICollection<Group> StudentGroups { get; set; } = new List<Group>(); // Группы, где он ученик
    }
}