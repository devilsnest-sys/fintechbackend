namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class LoanForecastDto
    {
        public int LoanId { get; set; }
        public decimal OutstandingPrincipal { get; set; }
        public decimal CurrentRegularInterest { get; set; }
        public decimal CurrentDelayInterest { get; set; }
        public DateTime AsOfDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsOverdue { get; set; }
        public int? DaysOverdue { get; set; }
        public decimal ProjectedInterestIfPaidToday { get; set; }
        public decimal ProjectedDelayInterestIfPaidToday { get; set; }
        public decimal TotalOutstandingIfPaidToday { get; set; }
    }
}
