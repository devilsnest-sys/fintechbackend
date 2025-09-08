using TscLoanManagement.TSCDB.Core.Enums;

namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class DdrStatusUpdateDto
    {
        public int DdrId { get; set; }
        public DDRStatus Status { get; set; }
    }
}
