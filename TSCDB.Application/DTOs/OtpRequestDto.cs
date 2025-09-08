namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class OtpRequestDto
    {
        //public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class VerifyOtpRequestDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
