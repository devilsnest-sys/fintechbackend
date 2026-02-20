using TscLoanManagement.TSCDB.Core.Enums;

namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class UpdateDealerStatusDto
    {
        public int DealerId { get; set; }
        public DealerStatus NewStatus { get; set; }
        public string? RejectionReason { get; set; }
    }
}
