using TscLoanManagement.TSCDB.Core.Domain.Loan;
using TscLoanManagement.TSCDB.Core.Domain.Dealer;
using TscLoanManagement.TSCDB.Core.Enums;
namespace TscLoanManagement.TSCDB.Core.Domain.DDR
{
    public class DDR
    {
        public int Id { get; set; }
        public string DDRNumber { get; set; }
        public DateTime DateOfWithdraw { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal? GSTOnProcessingFee { get; set; }
        public DateTime? ProcessingFeeReceivedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? SettledDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string UtrNumber { get; set; }
        public decimal? ProcessingFee { get; set; }

        // Dealer relationship
        public int DealerId { get; set; }
        public virtual TscLoanManagement.TSCDB.Core.Domain.Dealer.Dealer Dealer { get; set; }

        public string PurchaseSource { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InterestWaiver { get; set; }
        public decimal? DelayedROI { get; set; }

        public DDRStatus Status { get; set; }
        public bool IsSigned { get; set; }
        public decimal? SuggestedProcessingFee { get; set; }
        public decimal? SelectedProcessingFee { get; set; }

        // Hold information
        public string? HoldReason { get; set; }
        public string? HoldBy { get; set; }
        public DateTime? HoldDate { get; set; }

        // Rejection information
        public string? RejectedReason { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime? RejectionDate { get; set; }

        // Audit fields
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string? ApplicationId { get; set; }


        // Vehicle information - direct relationship
        public virtual VehicleInfo? VehicleInfo { get; set; }

        // Attachments for DDR
        //public virtual ICollection<LoanAttachment> Attachments { get; set; } = new List<LoanAttachment>();
    }

}
