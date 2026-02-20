namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class WaiverRequestDto
    {
        public string WaiverType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
