using System.ComponentModel.DataAnnotations;
using TscLoanManagement.TSCDB.Core.Domain.Loan;

namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class DdrCreateDto
    {

        public string DDRNumber { get; set; }


        public DateTime? DateOfWithdraw { get; set; }



        public decimal? Amount { get; set; }



        public decimal? InterestRate { get; set; }

        public decimal? GSTOnProcessingFee { get; set; }
        public DateTime? ProcessingFeeReceivedDate { get; set; }
        public decimal? SuggestedProcessingFee { get; set; }
        public decimal? SelectedProcessingFee { get; set; }


        public DateTime DueDate { get; set; }

        public string? UtrNumber { get; set; }
        public decimal? ProcessingFee { get; set; }


        public int DealerId { get; set; }

        public string? PurchaseSource { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InterestWaiver { get; set; }
        public decimal? DelayedROI { get; set; }

        public bool? IsSigned { get; set; }
        public string? ApplicationId { get; set; }


        // Vehicle Info
        public VehicleInfoDto? VehicleInfo { get; set; }

    }
}
