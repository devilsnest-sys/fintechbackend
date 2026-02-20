namespace TscLoanManagement.TSCDB.Application.DTOs.PurchaseSourceDocuments
{
    public class PurchaseSourceDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? LoanBidAmtPercentage { get; set; }
        public bool? IsApplicationNoRequired { get; set; }
    }
}
