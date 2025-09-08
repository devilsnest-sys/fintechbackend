using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IDDRDocumentService
    {
        Task<DDRDocumentUploadDto> UploadDocumentAsync(DDRDocumentUploadDto dto);
        Task<List<DDRDocumentUploadDto>> GetDocumentsByDdrIdAsync(int ddrId);
        Task<DDRDocumentUploadDto?> GetDocumentByIdAsync(int id);
    }
}
