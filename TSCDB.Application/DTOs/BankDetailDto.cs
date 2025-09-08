namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class BankDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } // Maybe AccountHolderName
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string IFSC { get; set; }
        public string BranchName { get; set; }
        public bool is_credit_account { get; set; }
    }
}
