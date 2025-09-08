namespace TscLoanManagement.TSCDB.Application.DTOs.LoanDocuments
{
    public class LoanDocumentMasterDto
    {
        public int Id { get; set; }
        public int DealerId { get; set; }
        public string DealerCode { get; set; }
        public string LoanNumber { get; set; }
        public string VehicleNumber { get; set; }
        public string Location { get; set; }
        public string RelationshipManager { get; set; }
        public string MakeModel { get; set; }
        public string ProcuredFrom { get; set; }
        public string VendorName { get; set; }
        public string Status { get; set; }
        public DateTime? DateOfDisbursement { get; set; }
        public int Tenure { get; set; }
        public string DocumentStatus { get; set; }
        public string Remarks { get; set; }
        public string DeliveryNoteNumber { get; set; }
        public bool DocumentReceived { get; set; }

        public List<LoanDocumentActivityDto> Activities { get; set; }
    }
}
