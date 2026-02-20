namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class UpdateRepresentativeDto
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int? RoleId { get; set; }
        public bool? IsRepresentative { get; set; }
        public string? Designation { get; set; }
    }
}
