using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IBankDetailService
    {
        Task<List<BankDetailDto>> GetAllAsync();
        Task<BankDetailDto> GetByIdAsync(int id);
        Task<BankDetailDto> CreateAsync(BankDetailDto dto);
        Task<BankDetailDto> UpdateAsync(int id, BankDetailDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
