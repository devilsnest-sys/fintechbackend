using TscLoanManagement.TSCDB.Core.Enums;

namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class DdrDto
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
        public string UtrNumber { get; set; }
        public decimal? ProcessingFee { get; set; }
        public int DealerId { get; set; }
        public string PurchaseSource { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InterestWaiver { get; set; }
        public decimal? DelayedROI { get; set; }
        public DDRStatus Status { get; set; }
        public bool IsSigned { get; set; }
        public string? HoldReason { get; set; }
        public string? HoldBy { get; set; }
        public DateTime? HoldDate { get; set; }
        public string? RejectedReason { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime? RejectionDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public VehicleInfoDto? VehicleInfo { get; set; }
        //public BuyerInfoDto? BuyerInfo { get; set; }
    }
}
