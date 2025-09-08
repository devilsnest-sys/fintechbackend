namespace TscLoanManagement.TSCDB.Core.Domain.LoanInstallment
{
    public class LoanInstallment
    {
        public int InstallmentId { get; set; }
        public int LoanId { get; set; }
        public DateTime PaidDate { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AdjustedToFee { get; set; } = 0;
        public decimal AdjustedToInterest { get; set; } = 0;
        public decimal AdjustedToPrincipal { get; set; } = 0;
        public decimal AdjustedToDelayInterest { get; set; } = 0;
        public string? Remarks { get; set; }
        public decimal DueFee { get; set; } = 0;
        public decimal DueInterest { get; set; } = 0;
        public decimal DueDelayInterest { get; set; } = 0;

        public LoanDetail Loan { get; set; }
    }
}
