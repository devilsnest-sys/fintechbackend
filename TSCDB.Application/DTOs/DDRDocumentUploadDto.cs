namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class DDRDocumentUploadDto
    {
        public int? Id { get; set; }
        public int DdrId { get; set; }                      // Foreign key to DDR
        public string DocumentType { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public IFormFile? Document { get; set; }
        public DateTime UploadedOn { get; set; }
    }
}
