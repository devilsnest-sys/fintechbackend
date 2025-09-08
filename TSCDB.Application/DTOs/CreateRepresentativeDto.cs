namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class CreateRepresentativeDto
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool? IsRepresentative { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string? Designation { get; set; }

    }
}
