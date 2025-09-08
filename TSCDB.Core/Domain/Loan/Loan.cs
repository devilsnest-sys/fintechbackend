using TscLoanManagement.TSCDB.Core.Domain.Dealer;
using TscLoanManagement.TSCDB.Core.Domain.LoanInstallment;
namespace TscLoanManagement.TSCDB.Core.Domain.Loan

{
    public class Loan
    {
        public int Id { get; set; }
        public string LoanNumber { get; set; }
        public DateTime DateOfWithdraw { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal? GSTOnProcessingFee { get; set; } // Newly added
        public DateTime? ProcessingFeeReceivedDate { get; set; } // Newly added
        public DateTime DueDate { get; set; }
        public DateTime? SettledDate { get; set; } // Newly added
        public decimal? SuggestedProcessingFee { get; set; }
        public decimal? SelectedProcessingFee { get; set; }

        public string? UtrNumber { get; set; }
        public decimal ProcessingFee { get; set; }

        public int DealerId { get; set; }
        public virtual TscLoanManagement.TSCDB.Core.Domain.Dealer.Dealer Dealer { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual VehicleInfo? VehicleInfo { get; set; }
        public virtual BuyerInfo? BuyerInfo { get; set; }

        public string PurchaseSource { get; set; } // Newly added
        public string InvoiceNumber { get; set; }  // Newly added
        public DateTime? InvoiceDate { get; set; } // Newly added
        public decimal? InterestWaiver { get; set; } // Newly added
        public decimal? DelayedROI { get; set; }
        public string? Status { get; set; }
        public int? LoanDetailId { get; set; }
        public LoanDetail LoanDetail { get; set; }
        public int? DDRId { get; set; } // Optional foreign key
        public virtual TscLoanManagement.TSCDB.Core.Domain.DDR.DDR DDR { get; set; }


        public virtual ICollection<LoanAttachment> Attachments { get; set; }
        public virtual ICollection<LoanPayment> LoanPayments { get; set; } // New
        public ICollection<LoanDocumentUpload> LoanDocumentUploads { get; set; }
    }

    public class LoanPayment
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentReceived { get; set; }
        public decimal InterestEarned { get; set; }
        public decimal DelayedInterestEarned { get; set; }
        public decimal PrincipalReceived { get; set; }

        public virtual Loan Loan { get; set; }
    }


}
