namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class BulkInstallmentDto
    {
        public int LoanId { get; set; }
        public DateTime PaidDate { get; set; }
        public decimal AmountPaid { get; set; }
        public  decimal ProcessingFee { get; set; }
        public decimal AdjustedToPrincipal { get; set; }
        public decimal AdjustedToInterest { get; set; }
        public decimal AdjustedToDelayInterest { get; set; }
        public decimal AdjustedToFee { get; set; }
        public decimal DueInterest { get; set; }
        public decimal DueFee { get; set; }
    }
}
