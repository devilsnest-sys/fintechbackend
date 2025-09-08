using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.DDR;
using TscLoanManagement.TSCDB.Core.Enums;

namespace TscLoanManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DdrController : ControllerBase
    {
        private readonly IDdrService _ddrService;

        public DdrController(IDdrService ddrService)
        {
            _ddrService = ddrService;
        }

        [HttpPost]
        public async Task<ActionResult<DdrResponseDto>> CreateDDR([FromBody] DdrCreateDto dto)
        {
            try
            {
                var result = await _ddrService.CreateDDRAsync(dto);
                return CreatedAtAction(nameof(GetDDR), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // ✅ Only return the message string

            }
        }

        /// <summary>
        /// Update an existing DDR
        /// </summary>
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DdrResponseDto>> UpdateDDR(int id, [FromBody] DdrUpdateDto dto)
        {
            try
            {
                var result = await _ddrService.UpdateDDRAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // ✅ Only return the message string

            }
        }

        /// <summary>
        /// Get DDR by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DdrResponseDto>> GetDDR(int id)
        {
            try
            {
                var result = await _ddrService.GetDDRByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all DDRs
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<DdrResponseDto>>> GetAllDDRs()
        {
            try
            {
                var result = await _ddrService.GetAllDDRsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get DDRs by dealer
        /// </summary>
        [HttpGet("dealer/{dealerId}")]
        public async Task<ActionResult<List<DdrResponseDto>>> GetDDRsByDealer(int dealerId)
        {
            try
            {
                var result = await _ddrService.GetDDRsByDealerAsync(dealerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get DDRs by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<DdrResponseDto>>> GetDDRsByStatus(DDRStatus status)
        {
            try
            {
                var result = await _ddrService.GetDDRsByStatusAsync(status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Approve DDR
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveDDR(int id)
        {
            try
            {
                await _ddrService.ApproveDDRAsync(id);
                return Ok(new { message = "DDR approved successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Reject DDR
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectDDR(int id, [FromBody] DdrRejectRequest request)
        {
            try
            {
                await _ddrService.RejectDDRAsync(id, request.RejectedBy, request.Reason);
                return Ok(new { message = "DDR rejected successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Hold DDR
        /// </summary>
        [HttpPost("{id}/hold")]
        public async Task<IActionResult> HoldDDR(int id, [FromBody] DdrHoldRequest request)
        {
            try
            {
                await _ddrService.HoldDDRAsync(id, request.HoldBy, request.Reason);
                return Ok(new { message = "DDR put on hold successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] DdrStatusUpdateDto dto)
        {
            try
            {
                var success = await _ddrService.UpdateDdrStatusAsync(dto);
                if (!success)
                    return NotFound(new { message = "DDR not found." });

                return Ok(new { message = "Status updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Delete DDR
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDDR(int id)
        {
            try
            {
                await _ddrService.DeleteDDRAsync(id);
                return Ok(new { message = "DDR deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("generate-ddr-number")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateDdrNumber()
        {
            try
            {
                var ddrNumber = await _ddrService.GenerateDdrNumberAsync();

                return Ok(new
                {
                    DdrNumber = ddrNumber
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error generating DDR number: " + ex.Message);
            }
        }

    }

}
