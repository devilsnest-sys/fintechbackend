namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class CreateUpdateLoanDto
    {
        public int Id { get; set; } // For update only, can be 0 or omitted on create
        public string? LoanNumber { get; set; } // Optional, auto-generated in backend
        public DateTime DateOfWithdraw { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal? GSTOnProcessingFee { get; set; }
        public DateTime? ProcessingFeeReceivedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? SettledDate { get; set; }

        public string? UtrNumber { get; set; }
        public decimal ProcessingFee { get; set; }

        public int DealerId { get; set; } // Use DealerId directly

        public string PurchaseSource { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InterestWaiver { get; set; }
        public decimal? DelayedROI { get; set; }

        public VehicleInfoDto VehicleInfo { get; set; }

        //public List<string> Attachments { get; set; } = new();
    }
}
