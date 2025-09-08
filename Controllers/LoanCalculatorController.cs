using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Application.Services.Implementation;
using TscLoanManagement.TSCDB.Core.Domain.LoanInstallment;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanCalculatorController : ControllerBase
    {
        //private readonly ILoanCalculationService _loanCalculationService;
        private readonly ILoanCalculationService loanCalculationService;

        public LoanCalculatorController(ILoanCalculationService _loanCalculationService)
        {
            loanCalculationService = _loanCalculationService;
        }


        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateLoan([FromBody] LoanCalculationRequestDto request)
        {
            var loanDetail = new LoanDetail
            {
                CustomerId = request.CustomerId,
                PrincipalAmount = request.PrincipalAmount,
                InterestRate = request.InterestRate,
                ProcessingFeeRate = request.ProcessingFeeRate,
                GSTPercent = request.GSTPercent,
                StartDate = request.StartDate,
                DueDays = request.DueDays,
                DelayInterestRate = request.DelayInterestRate
            };

            var installments = request.Installments
                .Select(i => (i.PaidDate, i.AmountPaid))
                .ToList();

            var result = await loanCalculationService.CreateLoanWithInstallmentsAsync(loanDetail, installments);

            return Ok(result);
        }

        [HttpGet("summary/{loanId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoanSummary(int loanId)
        {
            var result = await loanCalculationService.GetLoanSummaryAsync(loanId);
            if (result == null)
                return NotFound("Loan not found");

            return Ok(result);
        }

        [HttpPost("create-loan")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanRequestDto dto)
        {
            var loan = await loanCalculationService.CreateLoanAsync(dto);
            return Ok(loan);
        }

        [HttpPost("add-installment")]
        public async Task<IActionResult> AddInstallment([FromBody] AddInstallmentRequestDto dto)
        {
            var installment = await loanCalculationService.AddInstallmentAsync(dto);
            if (installment == null)
                return NotFound("Loan not found");

            return Ok(installment);
        }

        [HttpPost("waive/{loanId}")]
        [AllowAnonymous]
        public async Task<IActionResult> WaiveLoanComponent(int loanId, [FromBody] WaiverRequestDto request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            try
            {
                var result = await loanCalculationService.WaiveLoanComponentAsync(loanId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-pending-installment")]
        [AllowAnonymous]
        public async Task<IActionResult> AddPendingInstallment([FromBody] PendingInstallmentDto dto)
        {
            var result = await loanCalculationService.AddPendingInstallmentAsync(dto);
            return Ok(result);
        }

        [HttpPost("approve-pending-installment")]
        [AllowAnonymous]
        public async Task<IActionResult> ApprovePendingInstallment([FromBody] ApprovePendingInstallmentDto dto)
        {
            var result = await loanCalculationService.ApprovePendingInstallmentAsync(dto.PendingInstallmentId);
            if (result == null)
                return NotFound("Pending installment not found or already processed.");

            return Ok(result);
        }

        [HttpGet("pending-installments")]
        public async Task<IActionResult> GetPendingInstallments([FromQuery] int? loanId)
        {
            var result = await loanCalculationService.GetAllPendingInstallmentsAsync(loanId);
            return Ok(result);
        }


        //[HttpGet("accrued-interest/{loanId}")]
        //public async Task<IActionResult> GetAccruedInterest(int loanId)
        //{
        //    var result = await loanCalculationService.GetAccruedInterestSummaryAsync(loanId);
        //    if (result == null) return NotFound();
        //    return Ok(result);
        //}



    }
}
