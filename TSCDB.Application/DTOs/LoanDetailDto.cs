using TscLoanManagement.TSCDB.Core.Domain.LoanInstallment;

namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class LoanDetailDto
    {
        public int LoanId { get; set; }
        public int CustomerId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal ProcessingFeeRate { get; set; }
        public decimal GSTPercent { get; set; }
        public DateTime StartDate { get; set; }
        public int DueDays { get; set; }
        public decimal DelayInterestRate { get; set; }
        public string Status { get; set; } = "Active";
        public int? LoanDetailId { get; set; }
 

        public LoanFeeDto LoanFee { get; set; }
        public List<LoanInstallmentDto> Installments { get; set; } = new();
        public List<LoanInterestDto> Interests { get; set; } = new();
    }
}
