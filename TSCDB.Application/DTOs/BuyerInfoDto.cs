namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class BuyerInfoDto
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string IdentificationType { get; set; }
        public string IdentificationNumber { get; set; }
        public string BuyerSource { get; set; }
    }

}
