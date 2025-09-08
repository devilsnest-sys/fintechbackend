using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TscLoanManagement.TSCDB.Core.Domain.LoanInstallment
{
    public class LoanDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        public LoanFee LoanFee { get; set; }
        public List<LoanInstallment> Installments { get; set; } = new();
        public List<LoanInterest> Interests { get; set; } = new();
    }
}
