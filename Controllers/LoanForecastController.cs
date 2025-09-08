using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;

namespace TscLoanManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanForecastController : ControllerBase
    {
        private readonly ILoanForecastService _loanForecastService;

        public LoanForecastController(ILoanForecastService loanForecastService)
        {
            _loanForecastService = loanForecastService;
        }

        //[HttpPost("preview")]
        //public ActionResult<ForecastLoanResponseDto> GetLoanForecast([FromBody] ForecastLoanRequestDto dto)
        //{
        //    if (dto.DisbursementAmount <= 0 || dto.InterestRate <= 0)
        //        return BadRequest("Invalid input");

        //    var result = _loanForecastService.GetForecast(dto);
        //    return Ok(result);
        //}

        [HttpPost("preview")]
        [AllowAnonymous]
        public async Task<ActionResult<ForecastLoanResponseDto>> ForecastLoanAsync([FromBody] ForecastLoanRequestDto dto)
        {
            var result = await _loanForecastService.GetLoanForecastAsync(dto);
            return Ok(result);
        }
    }

}
