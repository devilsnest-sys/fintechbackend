namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class PaymentRecordDto
    {
        public DateTime? Date { get; set; }
        public decimal? Received { get; set; }
        public decimal? InterestEarned { get; set; }
        public decimal? DelayedInterestEarned { get; set; }
        public decimal? PrincipalReceived { get; set; }
    }
}
