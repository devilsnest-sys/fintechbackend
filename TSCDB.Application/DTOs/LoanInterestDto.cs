namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class LoanInterestDto
    {
        public int InterestId { get; set; }
        public int LoanId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string InterestType { get; set; } // Regular / Delay
        public decimal InterestAmount { get; set; }
    }
}
