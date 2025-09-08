using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.DTOs.LoanDocuments;
using TscLoanManagement.TSCDB.Application.Interfaces;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanDealerDocumentsController : ControllerBase
    {
        private readonly ILoanDealerDocumentService _loanDocService;

        public LoanDealerDocumentsController(ILoanDealerDocumentService loanDocService)
        {
            _loanDocService = loanDocService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] LoanDocumentMasterDto dto)
        {
            await _loanDocService.AddLoanDocumentAsync(dto);
            return Ok("Loan dealer document added.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _loanDocService.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{loanNumber}")]
        public async Task<IActionResult> GetByLoanNumber(string loanNumber)
        {
            var data = await _loanDocService.GetByLoanNumberAsync(loanNumber);
            return Ok(data);
        }

        [HttpPost("{loanNumber}/activity")]
        public async Task<IActionResult> AddActivity(string loanNumber, [FromBody] LoanDocumentActivityDto dto)
        {
            await _loanDocService.AddActivityAsync(loanNumber, dto);
            return Ok("Activity added.");
        }

        [HttpPost("bulk-upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> BulkUpload([FromForm] BulkLoanDocumentUploadDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("File is empty");

            await _loanDocService.BulkUploadAsync(request.File);
            return Ok("Bulk upload completed.");
        }

    }
}
