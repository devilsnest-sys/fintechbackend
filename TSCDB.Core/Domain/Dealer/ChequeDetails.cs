namespace TscLoanManagement.TSCDB.Core.Domain.Dealer
{
    public class ChequeDetails
    {
        public int Id { get; set; }
        public int DealerId { get; set; }
        public string BeneficiaryStatus { get; set; }
        public string BeneficiaryName { get; set; }
        public string ChequeNumber { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? DateHandover { get; set; }
        public string Location { get; set; }
        public string AttachmentUrl { get; set; }
        public bool IsENach { get; set; }
        public string? AccountType { get; set; } // Saving / Current / Others
        public string? MandateType { get; set; } // Fixed / Variable
        public decimal? MaxDebitAmount { get; set; } // Only for variable mandates
        public decimal? MandateAmount { get; set; } // For fixed mandates
        public string? Frequency { get; set; } // One-time / Monthly / Quarterly / As presented
        public DateTime? MandateStartDate { get; set; }
        public DateTime? MandateEndDate { get; set; }

        public string Remarks { get; set; }

        public virtual Dealer Dealer { get; set; }
    }
}
