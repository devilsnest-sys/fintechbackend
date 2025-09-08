namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class MultipleDocumentUploadDto
    {
        public string DocumentType { get; set; }
        public int DealerId { get; set; }
        public IFormFile Document { get; set; }
    }
}
