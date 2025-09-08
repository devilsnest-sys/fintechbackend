namespace TscLoanManagement.TSCDB.Core.Domain.Loan
{
    public class BuyerInfo
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string IdentificationType { get; set; } // Aadhar, PAN, etc.
        public string IdentificationNumber { get; set; }
        public string BuyerSource { get; set; }
        public virtual Loan Loan { get; set; }
        public int? DDRId { get; set; }
    }
}
