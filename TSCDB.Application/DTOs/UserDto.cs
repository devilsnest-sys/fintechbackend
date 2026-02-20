namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public bool? IsRepresentative { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? UserType { get; set; }
        public int? RoleId { get; set; }
        public RoleDto? Role { get; set; }
        public string? Designation { get; set; }
        public string? Token { get; set; }
    }
}
