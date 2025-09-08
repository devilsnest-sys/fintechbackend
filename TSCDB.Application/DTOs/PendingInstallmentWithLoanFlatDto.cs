using TscLoanManagement.TSCDB.Core.Domain.Loan;

namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class PendingInstallmentWithLoanFlatDto
    {
        public int Id { get; set; }
        public int? LoanId { get; set; }
        public int? LoanDetailId { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal? AmountPaid { get; set; }
        public string? Remarks { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public Loan Loan { get; set; }
    }
}
