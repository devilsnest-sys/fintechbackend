namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class WaiverDto
    {
        public int WaiverId { get; set; }
        public int LoanId { get; set; }
        public string WaiverType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
