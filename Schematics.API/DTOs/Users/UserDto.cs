namespace Schematics.API.DTOs.Users
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public  bool IsLockout { get; set; }
    }
}
