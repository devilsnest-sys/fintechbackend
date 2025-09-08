using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Core.Domain.DDR;
using TscLoanManagement.TSCDB.Core.Enums;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IDdrService
    {
        Task<DdrResponseDto> CreateDDRAsync(DdrCreateDto dto);
        Task<DdrResponseDto> UpdateDDRAsync(int id, DdrUpdateDto dto);
        Task<DdrResponseDto> GetDDRByIdAsync(int id);
        Task<List<DdrResponseDto>> GetAllDDRsAsync();
        Task<List<DdrResponseDto>> GetDDRsByDealerAsync(int dealerId);
        Task<List<DdrResponseDto>> GetDDRsByStatusAsync(DDRStatus status);
        Task ApproveDDRAsync(int ddrId);
        Task RejectDDRAsync(int ddrId, string rejectedBy, string reason);
        Task HoldDDRAsync(int ddrId, string holdBy, string reason);
        Task<bool> UpdateDdrStatusAsync(DdrStatusUpdateDto dto);
        Task DeleteDDRAsync(int id);
        Task<string> GenerateDdrNumberAsync();

    }
}
