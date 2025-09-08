namespace TscLoanManagement.TSCDB.Application.DTOs.LoanDocuments
{
    public class LoanDocumentActivityDto
    {
        public string ActivityType { get; set; }
        public string FromPerson { get; set; }
        public string ToPerson { get; set; }
        public DateTime Date { get; set; }
        public string DeliveryNoteNumber { get; set; }
    }
}
