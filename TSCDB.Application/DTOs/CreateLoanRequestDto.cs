namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class CreateLoanRequestDto
    {
        public int CustomerId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal ProcessingFeeRate { get; set; }
        public decimal GSTPercent { get; set; }
        public DateTime StartDate { get; set; }
        public int DueDays { get; set; }
        public decimal DelayInterestRate { get; set; }
        public string Status { get; set; }
    }
}
