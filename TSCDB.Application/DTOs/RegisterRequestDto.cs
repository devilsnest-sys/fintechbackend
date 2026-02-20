namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class RegisterRequestDto
    {
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? UserType { get; set; }
    }
}
