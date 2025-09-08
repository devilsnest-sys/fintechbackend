namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class LoanInstallmentDto
    {
        public int InstallmentId { get; set; }
        public int LoanId { get; set; }
        public DateTime PaidDate { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AdjustedToFee { get; set; }
        public decimal AdjustedToInterest { get; set; }
        public decimal AdjustedToPrincipal { get; set; }
        public decimal AdjustedToDelayInterest { get; set; }
        public string? Remarks { get; set; }

        public decimal DueFee { get; set; } = 0;
        public decimal DueInterest { get; set; } = 0;
        public decimal DueDelayInterest { get; set; } = 0;
    }
}
