using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DealerDetailsController : ControllerBase
    {
        private readonly IDealerDetailsService _dealerDetailsService;

        public DealerDetailsController(IDealerDetailsService dealerDetailsService)
        {
            _dealerDetailsService = dealerDetailsService;
        }

        [HttpPost("borrower")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveBorrowerDetails([FromBody] BorrowerDetailsDto dto)
        {
            var result = dto.Id > 0
                ? await _dealerDetailsService.UpdateBorrowerDetailsAsync(dto)
                : await _dealerDetailsService.SubmitBorrowerDetailsAsync(dto);

            return Ok(result);
        }

        [HttpPost("guarantor")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveGuarantorDetails([FromBody] GuarantorDetailsDto dto)
        {
            var result = dto.Id > 0
                ? await _dealerDetailsService.UpdateGuarantorDetailsAsync(dto)
                : await _dealerDetailsService.SubmitGuarantorDetailsAsync(dto);

            return Ok(result);
        }

        [HttpPost("cheque")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveChequeDetailsBulk([FromBody] List<ChequeDetailsDto> dtoList)
        {
            var results = new List<ChequeDetailsDto>();

            foreach (var dto in dtoList)
            {
                var result = dto.Id > 0
                    ? await _dealerDetailsService.UpdateChequeDetailsAsync(dto)
                    : await _dealerDetailsService.SubmitChequeDetailsAsync(dto);

                if (result != null)
                    results.Add(result);
            }

            return Ok(results);
        }



        [HttpPost("security-deposit")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveSecurityDepositDetails([FromBody] SecurityDepositDetailsDto dto)
        {
            var result = dto.Id > 0
                ? await _dealerDetailsService.UpdateSecurityDepositDetailsAsync(dto)
                : await _dealerDetailsService.SubmitSecurityDepositDetailsAsync(dto);

            return Ok(result);
        }

        [HttpPost("full-details")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveFullDealerDetails([FromBody] DealerFullDetailsDto dto)
        {
            var result = dto.Id > 0
                ? await _dealerDetailsService.UpdateFullDealerDetailsAsync(dto)
                : await _dealerDetailsService.SubmitFullDealerDetailsAsync(dto);

            if (!result.Success)
                return StatusCode(500, result);

            return Ok(result);
        }

        [HttpPost("borrower-guarantor")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveBorrowerAndGuarantorDetails([FromBody] BorrowerAndGuarantorDetailsDto dto)
        {

            var result = await _dealerDetailsService.SaveBorrowerAndGuarantorDetailsAsync(dto);

            if (!result.Success)
                return StatusCode(500, result);

            return Ok(result);
        }

    }
}