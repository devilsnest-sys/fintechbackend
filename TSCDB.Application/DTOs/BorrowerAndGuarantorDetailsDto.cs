namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class BorrowerAndGuarantorDetailsDto
    {
        public List<BorrowerDetailsDto> Borrowers { get; set; }
        public List<GuarantorDetailsDto> Guarantors { get; set; }
    }
}
