namespace TscLoanManagement.TSCDB.Core.Domain.LoanDocuments
{
    public class LoanDocumentActivity
    {
        public int Id { get; set; }
        public LoanDocumentMaster LoanDocumentMaster { get; set; }
        public string? ActivityType { get; set; }
        public string? FromPerson { get; set; }
        public string? ToPerson { get; set; }
        public DateTime? Date { get; set; }
        public string? DeliveryNoteNumber { get; set; }

        //public virtual LoanDocumentMaster? LoanDocumentMaster { get; set; }
    }
}
