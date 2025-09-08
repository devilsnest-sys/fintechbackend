namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class LoanSummaryDto
    {
        public int LoanId { get; set; }
        public decimal DisbursedAmount { get; set; }
        public decimal PrincipalReceived { get; set; }
        public decimal OutstandingPrincipal { get; set; }
        public decimal TotalInstallmentReceived { get; set; }

        public decimal RegularInterestReceived { get; set; }     // After waiver
        public decimal DelayInterestReceived { get; set; }        // After waiver

        public decimal ActualRegularInterestReceived { get; set; }   // Before waiver
        public decimal ActualDelayInterestReceived { get; set; }     // Before waiver

        public decimal ProcessingFee { get; set; }
        public decimal PaidProcessingFee { get; set; }
        public decimal GST { get; set; }

        public decimal AccruedRegularInterest { get; set; }
        public decimal AccruedDelayInterest { get; set; }
        public DateTime LastPaidDate { get; set; }
        public DateTime AccruedTillDate { get; set; }

        public decimal WaivedPrincipal { get; set; }
        public decimal WaivedInterest { get; set; }
        public decimal WaivedDelayInterest { get; set; }
        public decimal WaivedProcessingFee { get; set; }

        public decimal TotalProcessingFeeWithGST { get; set; }
        public decimal ProcessingFeeOutstanding => TotalProcessingFeeWithGST - PaidProcessingFee;

        public List<InstallmentDto> Installments { get; set; } = new();
    }




    public class InstallmentDto
    {
        public DateTime PaidDate { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AdjustedToPrincipal { get; set; }
        public decimal AdjustedToInterest { get; set; }
        public decimal AdjustedToDelayInterest { get; set; }
        public decimal AdjustedToFee { get; set; }
        public decimal DueFee { get; set; }
        public decimal DueInterest { get; set; }
        public decimal DueDelayInterest { get; set; }
    }
}
