namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class ForecastLoanRequestDto
    {
        public int DealerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PrincipalAmount { get; set; }
    }

}
