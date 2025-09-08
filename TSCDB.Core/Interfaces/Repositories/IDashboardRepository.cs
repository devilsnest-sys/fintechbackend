using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Core.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<DealerDashboardSummaryDto> GetDealerDashboardSummaryAsync(string dealerId);

    }
}
