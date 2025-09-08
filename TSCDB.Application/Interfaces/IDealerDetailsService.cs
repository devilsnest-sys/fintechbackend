using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Wrappers;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IDealerDetailsService
    {
        // Create operations
        Task<BorrowerDetailsDto> SubmitBorrowerDetailsAsync(BorrowerDetailsDto dto);
        Task<GuarantorDetailsDto> SubmitGuarantorDetailsAsync(GuarantorDetailsDto dto);
        Task<ChequeDetailsDto> SubmitChequeDetailsAsync(ChequeDetailsDto dto);
        Task<SecurityDepositDetailsDto> SubmitSecurityDepositDetailsAsync(SecurityDepositDetailsDto dto);
        Task<ApiResponse<string>> SubmitFullDealerDetailsAsync(DealerFullDetailsDto dto);

        // Update operations
        Task<BorrowerDetailsDto> UpdateBorrowerDetailsAsync(BorrowerDetailsDto dto);
        Task<GuarantorDetailsDto> UpdateGuarantorDetailsAsync(GuarantorDetailsDto dto);
        Task<ChequeDetailsDto> UpdateChequeDetailsAsync(ChequeDetailsDto dto);
        Task<SecurityDepositDetailsDto> UpdateSecurityDepositDetailsAsync(SecurityDepositDetailsDto dto);
        Task<ApiResponse<string>> UpdateFullDealerDetailsAsync(DealerFullDetailsDto dto);
        Task<ApiResponse<string>> SaveBorrowerAndGuarantorDetailsAsync(BorrowerAndGuarantorDetailsDto dto);

    }
}