namespace TscLoanManagement.TSCDB.Core.Domain.Authentication
{
    public class OtpVerification
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
