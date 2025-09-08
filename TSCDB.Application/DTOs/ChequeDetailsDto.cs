namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class ChequeDetailsDto
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
        public string? AccountType { get; set; }
        public string? MandateType { get; set; }
        public decimal? MaxDebitAmount { get; set; }
        public decimal? MandateAmount { get; set; }
        public string? Frequency { get; set; }
        public DateTime? MandateStartDate { get; set; }
        public DateTime? MandateEndDate { get; set; }

        public string Remarks { get; set; }
    }
}
