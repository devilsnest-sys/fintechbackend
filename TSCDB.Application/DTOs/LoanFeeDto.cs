namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class LoanFeeDto
    {
        public int LoanFeeId { get; set; }
        public int LoanId { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalFeeWithGST { get; set; }
    }
}
