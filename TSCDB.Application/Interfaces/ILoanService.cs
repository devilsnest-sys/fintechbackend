using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanDto>> GetAllLoansAsync();
        Task<IEnumerable<LoanDto>> GetLoansByDealerIdAsync(int dealerId);
        Task<LoanDto> GetLoanByIdAsync(int id);
        Task<LoanDto> CreateLoanAsync(CreateUpdateLoanDto loanDto);
        Task UpdateLoanAsync(CreateUpdateLoanDto loanDto);
        Task DeleteLoanAsync(int id);
        Task<bool> BulkUploadLoansAsync(IFormFile file);
        Task<decimal> CalculateInterestTillDateAsync(int loanId, DateTime? tillDate = null);
        Task<string> GenerateLoanNumberAsync();

    }
}
