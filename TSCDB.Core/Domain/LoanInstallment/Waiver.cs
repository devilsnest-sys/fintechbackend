namespace TscLoanManagement.TSCDB.Core.Domain.LoanInstallment
{
    public class Waiver
    {
        public int WaiverId { get; set; }
        public int LoanId { get; set; }
        public LoanDetail Loan { get; set; }

        public int? InstallmentId { get; set; }
        //public Installment Installment { get; set; }

        public string WaiverType { get; set; } = string.Empty; // "ProcessingFee", "Interest", "DelayInterest", "Principal"

        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
