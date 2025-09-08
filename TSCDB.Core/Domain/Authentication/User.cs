namespace TscLoanManagement.TSCDB.Core.Domain.Authentication
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public bool? IsRepresentative { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? UserType { get; set; }
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
        public string? Designation { get; set; }

        public ICollection<TscLoanManagement.TSCDB.Core.Domain.Dealer.Dealer> Dealers { get; set; } = new List<TscLoanManagement.TSCDB.Core.Domain.Dealer.Dealer>();
    }
}
