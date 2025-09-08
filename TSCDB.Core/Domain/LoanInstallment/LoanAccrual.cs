namespace TscLoanManagement.TSCDB.Core.Domain.LoanInstallment
{
    public class LoanAccrual
    {
        public int Id { get; set; }

        public int? LoanId { get; set; }
        public string? LoanNumber { get; set; }
        public DateTime? SnapshotDate { get; set; } // When this row was generated

        // Loan basic info
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? DelayInterestRate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Status { get; set; }

        // Accruals
        public decimal? AccruedInterest { get; set; }          // Till snapshot date
        public decimal? AccruedDelayedInterest { get; set; }   // Till snapshot date

        // Paid till date
        public decimal? TotalPrincipalPaid { get; set; }
        public decimal? TotalInterestPaid { get; set; }
        public decimal? TotalDelayedInterestPaid { get; set; }

        // Remaining balances
        public decimal? OutstandingPrincipal { get; set; }
        public decimal? OutstandingInterest { get; set; }
        public decimal? OutstandingDelayedInterest { get; set; }

        // Processing fee & waivers
        public decimal? ProcessingFee { get; set; }
        public decimal? GSTOnProcessingFee { get; set; }
        public decimal? TotalFeeWithGST { get; set; }
        public decimal? TotalWaivedAmount { get; set; } // from Waiver table
    }
}
