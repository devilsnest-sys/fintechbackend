namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class DdrRejectRequest
    {
        public string Reason { get; set; }

        public string RejectedBy { get; set; } = "Admin";
    }
}
