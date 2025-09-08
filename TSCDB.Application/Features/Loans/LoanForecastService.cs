using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Loans
{
    public class LoanForecastService : ILoanForecastService
    {
        private readonly IDealerRepository _dealerRepository;

        public LoanForecastService(IDealerRepository dealerRepository)
        {
            _dealerRepository = dealerRepository;
        }


        public async Task<ForecastLoanResponseDto> GetLoanForecastAsync(ForecastLoanRequestDto dto)
        {
            var dealer = await _dealerRepository.GetByIdAsync(dto.DealerId);
            if (dealer == null) return null;

            var startDate = dto.StartDate.Date;
            var endDate = dto.EndDate.Date;

            int interestDays = (endDate - startDate).Days + 1;

            var interestRate = (decimal)dealer.ROI;
            var delayInterestRate = (decimal)dealer.DelayROI;
            decimal principalAmount = dto.PrincipalAmount;

            decimal interest = Math.Round((principalAmount * interestRate * interestDays) / (365 * 100), 2);

            // Step 1: Calculate processing fee: ₹100 per ₹50,000
            //int slabs = (int)(principalAmount / 50000);
            decimal processingFee = CalculateProcessingFee(principalAmount);

            // Step 2: GST at 18%
            decimal gstOnProcessingFee = Math.Round(processingFee * 0.18M, 2);

            // Step 3: Total payable
            decimal totalPayable = principalAmount + interest + processingFee + gstOnProcessingFee;

            return new ForecastLoanResponseDto
            {
                DisbursementAmount = principalAmount,
                InterestCharge = interest,
                ProcessingFee = processingFee,
                gst = gstOnProcessingFee,
                ProcessingFeeWithGST = processingFee + gstOnProcessingFee,
                TotalPayableAmount = totalPayable,
                ROI = (decimal)dealer.ROI,                  // ← added
                DelayROI = (decimal)dealer.DelayROI
            };
        }

        private decimal CalculateProcessingFee(decimal principalAmount)
        {
            const decimal slabAmount = 50000m;
            const decimal slabFee = 100m;
            const decimal tolerancePercent = 0.20m;

            int fullSlabs = (int)(principalAmount / slabAmount);
            decimal remainder = principalAmount % slabAmount;
            decimal toleranceAmount = slabAmount * tolerancePercent; // 10,000

            if (remainder <= toleranceAmount)
            {
                return fullSlabs * slabFee;
            }
            else
            {
                return (fullSlabs + 1) * slabFee;
            }
        }


    }

}
