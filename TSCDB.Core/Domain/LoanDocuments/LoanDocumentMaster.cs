namespace TscLoanManagement.TSCDB.Core.Domain.LoanDocuments
{
    public class LoanDocumentMaster
    {
        public int Id { get; set; }
        public string DealerCode { get; set; }
        public virtual TscLoanManagement.TSCDB.Core.Domain.Dealer.Dealer Dealer { get; set; }
        public string? LoanNumber { get; set; }
        public string? VehicleNumber { get; set; }
        public string? Location { get; set; }
        public string? RelationshipManager { get; set; }
        public string? MakeModel { get; set; }
        public string? ProcuredFrom { get; set; }
        public string? VendorName { get; set; }
        public string? Status { get; set; }
        public DateTime? DateOfDisbursement { get; set; }
        public int? Tenure { get; set; }
        public string? DocumentStatus { get; set; }
        public string? Remarks { get; set; }

        public virtual ICollection<LoanDocumentActivity>? Activities { get; set; }
    }
}
