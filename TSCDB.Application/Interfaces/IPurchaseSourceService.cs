using TscLoanManagement.TSCDB.Application.DTOs.PurchaseSourceDocuments;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IPurchaseSourceService
    {
        Task<List<PurchaseSourceDto>> GetAllAsync();
        Task<PurchaseSourceDto> GetByIdAsync(int id);
        Task<PurchaseSourceDto> CreateAsync(PurchaseSourceDto dto);
        Task<PurchaseSourceDto> UpdateAsync(int id, PurchaseSourceDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
