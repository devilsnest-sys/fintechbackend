using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Core.Domain.LoanInstallment;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface ILoanCalculationService
    {
        Task<LoanDetail> CreateLoanWithInstallmentsAsync(LoanDetail loanDetail, List<(DateTime PaidDate, decimal AmountPaid)> installments);
        Task<LoanSummaryDto> GetLoanSummaryAsync(int loanId);

        Task<LoanDetail> CreateLoanAsync(CreateLoanRequestDto dto);

        Task<LoanInstallment> AddInstallmentAsync(AddInstallmentRequestDto dto);
        Task<LoanInstallment> AddBulkInstallmentAsync(BulkInstallmentDto dto);
        Task<bool> AddBulkLoanFeeAsync(BulkLoanFeeDto bulkDto);
        Task<WaiverDto> WaiveLoanComponentAsync(int loanId, WaiverRequestDto request);
        Task<PendingInstallment> AddPendingInstallmentAsync(PendingInstallmentDto dto);
        Task<LoanInstallment?> ApprovePendingInstallmentAsync(int pendingInstallmentId);
        Task<List<PendingInstallmentWithLoanFlatDto>> GetAllPendingInstallmentsAsync(int? loanId = null);


        //Task<AccruedInterestDto> GetAccruedInterestSummaryAsync(int loanId);

    }
}
