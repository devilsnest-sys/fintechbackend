using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IDealerService
    {
        Task<IEnumerable<DealerDto>> GetAllDealersAsync();
        Task<DealerDto> GetDealerByIdAsync(int id);
        Task<DealerDto> GetDealerByUserIdAsync(string userId);
        Task<DealerDto> CreateDealerAsync(DealerDto dealerDto);
        Task UpdateDealerAsync(DealerDto dealerDto);
        Task DeleteDealerAsync(int id);
        Task<bool> UpdateDealerStatusAsync(UpdateDealerStatusDto dto);
        Task<BulkUploadResultDto> BulkUploadDealersAsync(IFormFile excelFile);
        Task<string> GenerateDealerCodeAsync();
        Task<string> GenerateLoanProposalNoAsync();

    }
}