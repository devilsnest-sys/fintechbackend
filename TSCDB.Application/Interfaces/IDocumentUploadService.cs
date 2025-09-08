using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IDocumentUploadService
    {
        Task<DocumentUploadDto> UploadDocumentAsync(DocumentUploadDto dto);
        Task<DocumentUploadDto> GetByIdAsync(int id);
        Task<List<DocumentUploadDto>> GetByDealerIdAsync(int dealerId);


    }
}
