namespace TscLoanManagement.TSCDB.Application.DTOs.PurchaseSourceDocuments
{
    public class PurchaseSourceDocumentCreateManyDto
    {
        public int PurchaseSourceId { get; set; }
        public List<string> DocumentNames { get; set; } = new();
        public bool is_mandatory { get; set; }
    }
}
