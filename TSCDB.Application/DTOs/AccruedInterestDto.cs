namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class AccruedInterestDto
    {
        public int LoanId { get; set; }
        public decimal PrincipalRemaining { get; set; }
        public decimal AccruedRegularInterest { get; set; }
        public decimal AccruedDelayInterest { get; set; }
        public DateTime LastPaidDate { get; set; }
        public DateTime CalculationTillDate { get; set; }


    }
}
