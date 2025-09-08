namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class LoanDocumentUploadDto
    {
        public int? Id { get; set; }
        public int LoanId { get; set; } 
        public string? DocumentType { get; set; }
        public IFormFile Document { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedOn { get; set; }
    }
}
