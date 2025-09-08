namespace TscLoanManagement.TSCDB.Core.Domain.LoanInstallment
{
    public class PendingInstallment
    {
        public int Id { get; set; }
        public int? LoanId { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal? AmountPaid { get; set; }
        public string? Remarks { get; set; }
        public string? Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
