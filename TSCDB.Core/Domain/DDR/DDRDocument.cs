namespace TscLoanManagement.TSCDB.Core.Domain.DDR
{
    public class DDRDocument
    {
        public int Id { get; set; }
        public int DdrId { get; set; }
        public string DocumentType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedOn { get; set; }
    }
}
