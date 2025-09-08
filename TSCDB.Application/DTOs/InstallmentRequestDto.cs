namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class InstallmentRequestDto
    {
        public decimal Amount { get; set; }
        public DateTime PaidDate { get; set; }
        public string? Remarks { get; set; }
    }
}
