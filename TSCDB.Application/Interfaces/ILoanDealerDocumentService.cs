using TscLoanManagement.TSCDB.Application.DTOs.LoanDocuments;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface ILoanDealerDocumentService
    {
        Task AddLoanDocumentAsync(LoanDocumentMasterDto dto);
        Task<List<LoanDocumentMasterDto>> GetAllAsync();
        Task<LoanDocumentMasterDto> GetByLoanNumberAsync(string loanNumber);
        Task AddActivityAsync(string loanNumber, LoanDocumentActivityDto dto);
        Task BulkUploadAsync(IFormFile file);
    }
}
