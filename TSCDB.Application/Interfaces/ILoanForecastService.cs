using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface ILoanForecastService
    {
        //ForecastLoanResponseDto GetForecast(ForecastLoanRequestDto dto);
        //ForecastLoanResponseDto GetLoanForecast(ForecastLoanRequestDto dto);
        Task<ForecastLoanResponseDto> GetLoanForecastAsync(ForecastLoanRequestDto dto);

    }
}
