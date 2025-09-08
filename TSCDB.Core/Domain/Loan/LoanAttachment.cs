namespace TscLoanManagement.TSCDB.Core.Domain.Loan
{
    public class LoanAttachment
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public string? FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
