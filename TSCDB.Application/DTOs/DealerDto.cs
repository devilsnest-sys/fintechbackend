using TscLoanManagement.TSCDB.Core.Domain.Authentication;
using TscLoanManagement.TSCDB.Core.Domain.Loan;

namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class DealerDto
    {
        public int Id { get; set; }
        public string? DealerCode { get; set; }
        public string? LoanProposalNo { get; set; }
        public string DealershipName { get; set; }
        public string DealershipPAN { get; set; }
        public string GSTNo { get; set; }
        public string GSTRegStatus { get; set; }
        public string MSMERegistrationNo { get; set; }
        public string MSMEType { get; set; }
        public string MSMEStatus { get; set; }
        public string BusinessCategory { get; set; }
        public string BusinessType { get; set; }
        public string Entity { get; set; } // TypeOfEntity
        public string ContactNo { get; set; }
        public string? AlternativeContactNo { get; set; }
        public string EmailId { get; set; }
        public string? AlternativeEmailId { get; set; }

        public string ShopAddress { get; set; }
        public string? ShopPinCode { get; set; }
        public string ?ShopState { get; set; }
        public string ParkingYardAddress { get; set; }
        public string? ParkingYardPinCode { get; set; }
        public string? ParkingYardState { get; set; }
        public string? ParkingYardCity { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }

        public string OfficeStatus { get; set; }
        public DateTime? AgreementDate { get; set; }
        public DateTime? AgreementExpiryDate { get; set; }

        public string ParkingStatus { get; set; }
        public DateTime? ParkingAgreementDate { get; set; }
        public DateTime? ParkingAgreementExpiryDate { get; set; }

        public DateTime? DateOfIncorporation { get; set; }
        public DateTime? DateOfFacilityAgreement { get; set; }


        public int? RelationshipManagerId { get; set; }
        public string? RelationshipManagerName { get; set; } // For display in responses

        public string Status { get; set; }


        //public string UserId { get; set; }

        //public UpdateDealerStatusDto Status { get; set; }

        public List<ChequeDetailsDto> ChequeDetails { get; set; }
        public List<BorrowerDetailsDto> BorrowerDetails { get; set; }
        public List<GuarantorDetailsDto> GuarantorDetails { get; set; }
        public List<SecurityDepositDetailsDto> SecurityDepositDetails { get; set; }
        public List<DocumentUploadDto> DocumentUploads { get; set; }
    }
}
