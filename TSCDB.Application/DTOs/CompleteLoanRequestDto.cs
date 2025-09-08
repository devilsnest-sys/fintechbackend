namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class CompleteLoanRequestDto
    {
        public LoanDto LoanInfo { get; set; }
        public CreateLoanRequestDto CalculationInfo { get; set; }
    }
}
