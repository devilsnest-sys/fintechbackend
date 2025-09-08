using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanDocumentUploadController : ControllerBase
    {
        private readonly ILoanDocumentService _loanDocumentService;
        private readonly ILogger<LoanDocumentUploadController> _logger;
        private readonly IWebHostEnvironment _env;

        public LoanDocumentUploadController(
            ILoanDocumentService loanDocumentService,
            ILogger<LoanDocumentUploadController> logger,
            IWebHostEnvironment env)
        {
            _loanDocumentService = loanDocumentService;
            _logger = logger;
            _env = env;
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var folderPath = Path.Combine("Uploads", "LoanDocuments");
            var fullFolderPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), folderPath);
            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(fullFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(folderPath, fileName).Replace("\\", "/");
        }

        [HttpPost("upload-multiple-documents")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadMultipleDocumentsAsObjects(
           [FromForm] List<LoanDocumentUploadDto> documents)
        {
            if (documents == null || documents.Count == 0)
                return BadRequest("No documents provided.");

            try
            {
                var uploadedDocuments = new List<LoanDocumentUploadDto>();

                foreach (var doc in documents)
                {
                    if (doc == null || doc.Document == null || doc.Document.Length == 0 || string.IsNullOrEmpty(doc.DocumentType))
                    {
                        _logger.LogWarning("Skipping document with null or empty fields");
                        continue;
                    }

                    var filePath = await SaveFileAsync(doc.Document);

                    var uploadDto = new LoanDocumentUploadDto
                    {
                        LoanId = doc.LoanId,
                        DocumentType = doc.DocumentType,
                        FileName = doc.Document.FileName,
                        FilePath = filePath,
                        UploadedOn = DateTime.UtcNow
                    };

                    var result = await _loanDocumentService.UploadDocumentAsync(uploadDto);
                    uploadedDocuments.Add(result);
                }

                return Ok(uploadedDocuments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading multiple loan documents");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("by-loan/{loanId}")]
        public async Task<IActionResult> GetDocumentsByLoanId(int loanId)
        {
            try
            {
                var documents = await _loanDocumentService.GetDocumentsByLoanIdAsync(loanId);
                if (documents == null || documents.Count == 0)
                    return NotFound(new { message = $"No documents found for Loan ID {loanId}" });

                foreach (var doc in documents)
                {
                    if (!string.IsNullOrEmpty(doc.FilePath))
                    {
                        doc.FilePath = $"{Request.Scheme}://{Request.Host}/{doc.FilePath}";
                    }
                }
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching loan documents");
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        [HttpGet("view/{id}")]
        public async Task<IActionResult> ViewDocument(int id)
        {
            var document = await _loanDocumentService.GetDocumentByIdAsync(id);
            if (document == null)
                return NotFound(new { message = $"Document with ID {id} not found" });

            return Ok(document);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var document = await _loanDocumentService.GetDocumentByIdAsync(id);
            if (document == null)
                return NotFound(new { message = $"Document with ID {id} not found" });

            var fullPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), document.FilePath);
            if (!System.IO.File.Exists(fullPath))
                return NotFound(new { message = $"File not found on server for Document ID {id}" });

            var contentType = "application/octet-stream";
            var fileName = document.FileName;

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(fileBytes, contentType, fileName);
        }

    }
}
