namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class BulkLoanFeeDto
    {
        public int LoanId { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalFeeWithGST { get; set; }
        //public DateTime? FeeReceivedDate { get; set; }
    }
}
