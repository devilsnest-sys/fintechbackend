using TscLoanManagement.TSCDB.Core.Domain.LoanInstallment;

namespace TscLoanManagement.TSCDB.Core.Domain.Loan
{
    public class LoanDocumentUpload
    {
        public int Id { get; set; }

        public int LoanId { get; set; }
        public Loan Loan { get; set; }

        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
    }
}
