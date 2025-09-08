using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface ILoanDocumentService
    {
        Task<LoanDocumentUploadDto> UploadDocumentAsync(LoanDocumentUploadDto dto);
        Task<List<LoanDocumentUploadDto>> GetDocumentsByLoanIdAsync(int loanId);
        Task<LoanDocumentUploadDto> GetDocumentByIdAsync(int id);

    }
}
