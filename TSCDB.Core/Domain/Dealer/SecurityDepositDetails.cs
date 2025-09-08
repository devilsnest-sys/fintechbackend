    namespace TscLoanManagement.TSCDB.Core.Domain.Dealer
    {
        public class SecurityDepositDetails
        {
            public int Id { get; set; }
            public int DealerId { get; set; }
            public string? Status { get; set; }
            public decimal Amount { get; set; }
            public string? UTRNumber { get; set; }
            public DateTime? DateReceived { get; set; }
            public DateTime? DateRefunded { get; set; }
            public string? AttachmentUrl { get; set; }
            public string? Remarks { get; set; }

            public string? CIBILOfEntity { get; set; }

            public decimal? TotalSanctionLimit { get; set; }
            public decimal? ROI { get; set; }
            public decimal? ROIPerLakh { get; set; }
            public decimal? DelayROI { get; set; }
        public DateTime? AgreementDate { get; set; }

        public decimal? ProcessingFee { get; set; }
            public decimal? ProcessingCharge { get; set; }
            public decimal? GSTOnProcessingCharge { get; set; }
            public decimal? DocumentationCharge { get; set; }
            public decimal? GSTOnDocCharges { get; set; }
            public DateTime RegisteredDate { get; internal set; }
            public string? RejectionReason { get; set; }

            public bool IsActive { get; set; }

            public virtual Dealer Dealer { get; set; }
        }
    }
