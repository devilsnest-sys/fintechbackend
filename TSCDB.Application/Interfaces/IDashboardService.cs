using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DealerDashboardSummaryDto> GetSummaryAsync(string dealerId);
    }
}
