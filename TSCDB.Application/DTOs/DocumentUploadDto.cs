namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class DocumentUploadDto
    {
        public int? DealerId { get; set; }

        public string DocumentType { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedOn { get; set; }
        public string FileName { get; set; }
    }
}
