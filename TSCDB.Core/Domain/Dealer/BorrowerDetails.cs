namespace TscLoanManagement.TSCDB.Core.Domain.Dealer
{
    public class BorrowerDetails
    {
        public int Id { get; set; }
        public int DealerId { get; set; }
        public Dealer Dealer { get; set; }

        public string PersonType { get; set; } // New
        public string Name { get; set; }
        public string PAN { get; set; }
        public string AadharNo { get; set; } // New
        public DateTime? DateOfBirth { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }

        public string FatherName { get; set; }
        public string RelationshipWithEntity { get; set; }

        public string CurrentAddress { get; set; }
        public string? BorrowerCPinCode { get; set; }
        public string? BorrowerCState { get; set; }
        public string PermanentAddress { get; set; }
        public string? BorrowerPPincode { get; set; }
        public string? BorrowerPState { get; set; }

        public string AddressStatus { get; set; } // New
        public DateTime? AddressAgreementDate { get; set; } // New
        public DateTime? AddressAgreementExpiryDate { get; set; } // New

        public int? CIBILScore { get; set; }
        public bool IsActive { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
        public string? BorrowerType { get; set; }

    }
}
