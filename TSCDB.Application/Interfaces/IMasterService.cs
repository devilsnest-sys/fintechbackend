using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Core.Domain.Masters;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IMasterService<T> where T : IMasterEntity
    {
        Task<List<MasterDto>> GetAllAsync();
        Task<MasterDto> GetByIdAsync(int id);
        Task<MasterDto> CreateAsync(MasterDto dto);
        Task<MasterDto> UpdateAsync(int id, MasterDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
