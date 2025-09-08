using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Features.Dealers;
using TscLoanManagement.TSCDB.Application.Interfaces;

namespace TscLoanManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetAllLoans()
        {
            var loans = await _loanService.GetAllLoansAsync();
            return Ok(loans);
        }

        [HttpGet("dealer/{dealerId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoansByDealerId(int dealerId)
        {
            var loans = await _loanService.GetLoansByDealerIdAsync(dealerId);
            return Ok(loans);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<LoanDto>> GetLoanById(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            if (loan == null)
                return NotFound();

            return Ok(loan);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LoanDto>> CreateLoan(CreateUpdateLoanDto loanDto)
        {
            var createdLoan = await _loanService.CreateLoanAsync(loanDto);
            return CreatedAtAction(nameof(GetLoanById), new { id = createdLoan.Id }, createdLoan);
        }


        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> UpdateLoan(int id, CreateUpdateLoanDto loanDto)
        {
            if (id != loanDto.Id)
                return BadRequest();

            try
            {
                await _loanService.UpdateLoanAsync(loanDto);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLoan(int id)
        {
            try
            {
                await _loanService.DeleteLoanAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("bulk-upload")]
        [AllowAnonymous]
        public async Task<ActionResult> BulkUploadLoans(IFormFile file)
        {
            try
            {
                var result = await _loanService.BulkUploadLoansAsync(file);
                return Ok(new { success = result });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("generate-codes")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateCodes()
        {
            try
            {
                var loanNumber = await _loanService.GenerateLoanNumberAsync();

                return Ok(new
                {
                    LoanNumber = loanNumber
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error generating codes: " + ex.Message);
            }
        }
    }
}
