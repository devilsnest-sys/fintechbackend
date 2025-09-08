using TscLoanManagement.TSCDB.Core.Domain.LoanInstallment;

namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class LoanDto
    {
        public int Id { get; set; }
        public string LoanNumber { get; set; }
        public DateTime DateOfWithdraw { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int? LoanDetailId { get; set; }
        public decimal? GSTOnProcessingFee { get; set; }
        public DateTime? ProcessingFeeReceivedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? SettledDate { get; set; }

        public string? UtrNumber { get; set; }
        public decimal ProcessingFee { get; set; }

        public DealerDto Dealer { get; set; } // via navigation

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public string PurchaseSource { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InterestWaiver { get; set; }
        public decimal? DelayedROI { get; set; }
        //public int? LoanDetailId { get; set; }
        public LoanDetail LoanDetail { get; set; }

        public VehicleInfoDto VehicleInfo { get; set; }
        public DdrResponseDto DDR { get; set; }
        //public BuyerInfoDto BuyerInfo { get; set; }

        public List<string> Attachments { get; set; } = new();
        public List<LoanPaymentDto> LoanPayments { get; set; } = new();
    }

}

public class LoanPaymentDto
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal PaymentReceived { get; set; }
    public decimal InterestEarned { get; set; }
    public decimal DelayedInterestEarned { get; set; }
    public decimal PrincipalReceived { get; set; }
}
