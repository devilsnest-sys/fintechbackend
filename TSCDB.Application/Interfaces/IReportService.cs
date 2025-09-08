using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateReportAsync(ReportRequestDto request);
    }
}
