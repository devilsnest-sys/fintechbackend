namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class DdrHoldRequest
    {
        public string Reason { get; set; }

        public string HoldBy { get; set; } = "Admin";
    }
}
