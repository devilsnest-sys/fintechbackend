namespace TscLoanManagement.TSCDB.Core.Domain.Dealer
{
    public class DocumentUpload
    {
        public int Id { get; set; }
        public int? DealerId { get; set; }
        public int? BorrowerDetailsId { get; set; }
        public int? GuarantorDetailsId { get; set; }

        public string DocumentType { get; set; } // e.g. DealershipPAN, GSTCertificate, BorrowerAadhar, etc.
        public string FilePath { get; set; }
        public DateTime UploadedOn { get; set; }

        public virtual Dealer Dealer { get; set; }
        public virtual BorrowerDetails BorrowerDetails { get; set; }
        public virtual GuarantorDetails GuarantorDetails { get; set; }
    }
}
