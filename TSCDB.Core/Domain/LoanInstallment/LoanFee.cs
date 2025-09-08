namespace TscLoanManagement.TSCDB.Core.Domain.LoanInstallment
{
    public class LoanFee
    {
        public int LoanFeeId { get; set; }
        public int LoanId { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalFeeWithGST { get; set; }

        public LoanDetail Loan { get; set; }
    }
}
