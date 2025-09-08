namespace TscLoanManagement.TSCDB.Core.Domain.Dealer
{
    public class GuarantorDetails
    {
        public int? Id { get; set; }
        public int? DealerId { get; set; }
        public Dealer? Dealer { get; set; }

        public string? PersonType { get; set; } // New
        public string? Name { get; set; }
        public string? PAN { get; set; }
        public string? AadharNo { get; set; } // New
        public DateTime? DateOfBirth { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }

        public string? FatherName { get; set; }
        public string? RelationshipWithBorrower { get; set; }

        public string? CurrentAddress { get; set; }
        public string? GuarantorCPinCode { get; set; }
        public string? GuarantorCState { get; set; }
        public string? PermanentAddress { get; set; }
        public string? GuarantorPPincode { get; set; }
        public string? GuarantorPState { get; set; }
        public string? AddressStatus { get; set; } // New
        public DateTime? AddressAgreementDate { get; set; } // New
        public DateTime? AddressAgreementExpiryDate { get; set; } // New

        public int? CIBILScore { get; set; }
        public bool? IsActive { get; internal set; }
        public DateTime? CreatedAt { get; internal set; }
        public string? GuarantorType { get; set; }

    }
}
