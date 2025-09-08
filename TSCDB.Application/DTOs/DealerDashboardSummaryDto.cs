namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class DealerDashboardSummaryDto
    {
        public int? DealerId { get; set; }
        public string? DealershipName { get; set; }
        public decimal? TotalSanctionLimit { get; set; }
        public decimal? TotalLoanAmount { get; set; }
        public decimal? TotalPrincipalPaid { get; set; }
        public decimal? TotalWaiversAmount { get; set; }
        public decimal? AvailableLimit { get; set; }
        public decimal? TotalAmountPending { get; set; }
        public decimal? UtilizationPercent { get; set; }
        public decimal? TotalInterestEarned { get; set; }
        public decimal? DisbursementPerDay { get; set; }

        // Newly added fields
        public decimal? TotalDelayedInterestEarned { get; set; }
        public decimal? AccruedInterestActive { get; set; }
        public decimal? AccruedDelayedInterestActive { get; set; }
        public decimal? TotalProcessingFees { get; set; }
        public decimal? LoansSanctionedPerDay { get; set; }
        public decimal? ProcessingFeeEarnedActive { get; set; }
        public decimal? TotalOutstandingActive { get; set; }
        public decimal? PrincipalOutstandingActive { get; set; }
        public decimal? InterestEarnedActive { get; set; }
        public decimal? DelayedInterestEarnedActive { get; set; }
        public decimal? ProcessingFeesReceivedActive { get; set; }
        public int? ActiveLoansCount { get; set; }
        public int? CLosedLoansCount { get; set; }

    }
}
