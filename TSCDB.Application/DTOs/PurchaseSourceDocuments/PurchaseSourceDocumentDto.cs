namespace TscLoanManagement.TSCDB.Application.DTOs.PurchaseSourceDocuments
{
    public class PurchaseSourceDocumentDto
    {
        public int Id { get; set; }
        public int PurchaseSourceId { get; set; }
        public string? DocumentName { get; set; }
        public bool is_mandatory { get; set; }
    }
}
