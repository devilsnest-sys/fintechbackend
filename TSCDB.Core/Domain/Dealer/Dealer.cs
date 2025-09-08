using TscLoanManagement.TSCDB.Core.Domain.Authentication;
using TscLoanManagement.TSCDB.Core.Domain.Loan;
using TscLoanManagement.TSCDB.Core.Enums;

namespace TscLoanManagement.TSCDB.Core.Domain.Dealer
{
    public class Dealer
    {
        public int Id { get; set; }
        public string? DealerCode { get; set; }
        public string? LoanProposalNo { get; set; }
        public string? DealershipName { get; set; }
        public string? DealershipPAN { get; set; }
        public string? GSTNo { get; set; }
        public string? GSTRegStatus { get; set; }
        public string? MSMERegistrationNo { get; set; }
        public string? MSMEType { get; set; }
        public string? MSMEStatus { get; set; }
        public string? BusinessCategory { get; set; }
        public string? BusinessType { get; set; }
        public string? Entity { get; set; } // TypeOfEntity
        public string? ContactNo { get; set; }
        public string? AlternativeContactNo { get; set; }
        public string? EmailId { get; set; }
        public string? AlternativeEmailId { get; set; }

        public string? ShopAddress { get; set; }
        public string? ShopPinCode { get; set; }
        public string? ShopState { get; set; }
        public string? ParkingYardAddress { get; set; }
        public string? ParkingYardPinCode{ get; set; }
        public string? ParkingYardState { get; set; }
        public string? ParkingYardCity { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }

        public string? OfficeStatus { get; set; }
        public DateTime? AgreementDate { get; set; }
        public DateTime? AgreementExpiryDate { get; set; }

        public string? ParkingStatus { get; set; }
        public DateTime? ParkingAgreementDate { get; set; }
        public DateTime? ParkingAgreementExpiryDate { get; set; }

        public DateTime? DateOfIncorporation { get; set; }
        public DateTime? DateOfFacilityAgreement { get; set; }
        public string? CIBILOfEntity { get; set; }

        public decimal? TotalSanctionLimit { get; set; }
        public decimal? ROI { get; set; }
        public decimal? ROIPerLakh { get; set; }
        public decimal? DelayROI { get; set; }

        public decimal? ProcessingFee { get; set; }
        public decimal? ProcessingCharge { get; set; }
        public decimal? GSTOnProcessingCharge { get; set; }
        public decimal? DocumentationCharge { get; set; }
        public decimal? GSTOnDocCharges { get; set; }

        public int? RelationshipManagerId { get; set; } // Foreign key
        public User? RelationshipManager { get; set; }
        public DealerStatus? Status { get; set; } = DealerStatus.Pending;
        public string? RejectionReason { get; set; }

        public bool? IsActive { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public virtual ICollection<TscLoanManagement.TSCDB.Core.Domain.Loan.Loan> Loans { get; set; }
        public ICollection<BorrowerDetails> BorrowerDetails { get; set; }
        public ICollection<GuarantorDetails> GuarantorDetails { get; set; }
        public ICollection<ChequeDetails> ChequeDetails { get; set; }
        public ICollection<SecurityDepositDetails> SecurityDepositDetails { get; set; }
        public ICollection<DocumentUpload> DocumentUploads { get; set; }
        public DateTime? RegisteredDate { get; internal set; }
    }
}
