namespace OnlineSchool.Shared.DTOs
{
    public class GroupDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProgramName { get; set; }
        public string TeacherName { get; set; }
        public List<UserDto> Students { get; set; } = new List<UserDto>();
    }
}