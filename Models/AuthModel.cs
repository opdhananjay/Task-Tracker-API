using devops.Enums;

namespace devops.Models
{
    public class Registration
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string? Department { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmployeeId { get; set; }
    }

    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
