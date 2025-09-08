namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class PendingInstallmentDto
    {
        public int LoanId { get; set; }
        public DateTime PaidDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string? Remarks { get; set; }
    }
}
