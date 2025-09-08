namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class AddInstallmentRequestDto
    {
        public int LoanId { get; set; }
        public DateTime PaidDate { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
