namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class ReportRequestDto
    {
        public string? ReportType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
