using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentUploadController : ControllerBase
    {
        private readonly IDocumentUploadService _documentService;
        private readonly IDealerRepository _dealerRepository;
        private readonly ILogger<DocumentUploadController> _logger;
        private readonly string _webRootPath;

        public DocumentUploadController(
            IDocumentUploadService documentService,
            IDealerRepository dealerRepository,
            ILogger<DocumentUploadController> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _documentService = documentService;
            _dealerRepository = dealerRepository;
            _logger = logger;
            _webRootPath = webHostEnvironment?.WebRootPath;

            // Log if WebRootPath is null for debugging purposes
            if (_webRootPath == null)
            {
                _logger.LogWarning("WebRootPath is null in DocumentUploadController constructor");
            }
        }

        [HttpPost("upload")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadDto dto, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                // Save file to disk or blob and set path
                var filePath = await SaveFileAsync(file);
                dto.FilePath = filePath;
                dto.FileName = file.FileName; // Store original filename

                var result = await _documentService.UploadDocumentAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file), "File cannot be null");

            // Ensure we have a valid web root path
            var basePath = !string.IsNullOrEmpty(_webRootPath)
                ? _webRootPath
                : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Create uploads directory if it doesn't exist
            var uploadsDir = Path.Combine(basePath, "uploads");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            // Create a unique filename to prevent collisions
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            // Save the file
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path for storage in database
            return $"/uploads/{fileName}";
        }

        // GET: api/DocumentUpload/view/{id}
        [HttpGet("view/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ViewDocument(int id)
        {
            try
            {
                var document = await _documentService.GetByIdAsync(id);
                if (document == null)
                    return NotFound("Document not found in database");

                return await GetDocumentView(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error viewing document with ID {id}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/DocumentUpload/view-by-dealer/{dealerId}/{documentType}
        [HttpGet("view-by-dealer/{dealerId}/{documentType}")]
        [AllowAnonymous]
        public async Task<IActionResult> ViewDocumentByDealer(int dealerId, string documentType)
        {
            try
            {
                var documents = await _documentService.GetByDealerIdAsync(dealerId);
                if (documents == null || !documents.Any())
                    return NotFound($"No documents found for dealer ID {dealerId}");

                // Find the document with the specified documentType (get the most recent one if multiple exist)
                var document = documents
                    .Where(d => d.DocumentType == documentType)
                    .OrderByDescending(d => d.UploadedOn)
                    .FirstOrDefault();

                if (document == null)
                    return NotFound($"No document of type '{documentType}' found for dealer ID {dealerId}");

                return await GetDocumentView(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error viewing document for dealer ID {dealerId} and document type {documentType}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Helper method to handle common view document logic
        private async Task<IActionResult> GetDocumentView(DocumentUploadDto document)
        {
            if (string.IsNullOrEmpty(document.FilePath))
                return BadRequest("Document has no associated file path");

            var relativePath = document.FilePath.TrimStart('/');

            // Ensure we have a valid web root path
            var basePath = !string.IsNullOrEmpty(_webRootPath)
                ? _webRootPath
                : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var fullPath = Path.Combine(basePath, relativePath);

            // Check if file exists
            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning($"File not found at path: {fullPath}");
                return NotFound($"File not found on disk. Path checked: {relativePath}");
            }

            var fileName = Path.GetFileName(document.FilePath);
            var fileUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}";

            return Ok(new
            {
                Url = fileUrl,
                FileName = document.FileName ?? fileName,
                DocumentType = document.DocumentType,
                UploadedOn = document.UploadedOn
            });
        }

        // GET: api/DocumentUpload/download/{id}
        [HttpGet("download/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            try
            {
                var document = await _documentService.GetByIdAsync(id);
                if (document == null)
                    return NotFound("Document not found in database");

                return await DownloadDocumentFile(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading document with ID {id}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/DocumentUpload/download-by-dealer/{dealerId}/{documentType}
        [HttpGet("download-by-dealer/{dealerId}/{documentType}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadDocumentByDealer(int dealerId, string documentType)
        {
            try
            {
                var documents = await _documentService.GetByDealerIdAsync(dealerId);
                if (documents == null || !documents.Any())
                    return NotFound($"No documents found for dealer ID {dealerId}");

                // Find the document with the specified documentType (get the most recent one if multiple exist)
                var document = documents
                    .Where(d => d.DocumentType == documentType)
                    .OrderByDescending(d => d.UploadedOn)
                    .FirstOrDefault();

                if (document == null)
                    return NotFound($"No document of type '{documentType}' found for dealer ID {dealerId}");

                return await DownloadDocumentFile(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading document for dealer ID {dealerId} and document type {documentType}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Helper method to handle common download document logic
        private async Task<IActionResult> DownloadDocumentFile(DocumentUploadDto document)
        {
            if (string.IsNullOrEmpty(document.FilePath))
                return BadRequest("Document has no associated file path");

            var relativePath = document.FilePath.TrimStart('/');

            // Ensure we have a valid web root path
            var basePath = !string.IsNullOrEmpty(_webRootPath)
                ? _webRootPath
                : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var fullPath = Path.Combine(basePath, relativePath);

            // Check if file exists
            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning($"File not found at path: {fullPath}");
                return NotFound($"File not found on disk. Path checked: {relativePath}");
            }

            // Determine content type based on file extension
            var contentType = GetContentType(Path.GetExtension(fullPath));

            // Read file and return
            var memory = new MemoryStream();
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            // Use the original filename if available, otherwise use the current filename
            var fileName = document.FileName ?? Path.GetFileName(fullPath);

            return File(memory, contentType, fileName);
        }

        [HttpGet("by-dealer/{dealerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDocumentsByDealer(int dealerId)
        {
            try
            {
                var documents = await _documentService.GetByDealerIdAsync(dealerId);

                // Add full URLs for each document
                foreach (var doc in documents)
                {
                    if (!string.IsNullOrEmpty(doc.FilePath))
                    {
                        doc.FilePath = $"{Request.Scheme}://{Request.Host}{doc.FilePath}";
                    }
                }

                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting documents for dealer ID {dealerId}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("upload-multiple")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadMultipleDocuments(
            [FromForm] List<IFormFile> files,
            [FromForm] List<string> documentTypes,
            [FromForm] int dealerId)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

            if (files.Count != documentTypes.Count)
                return BadRequest("Mismatch between files and document types.");

            try
            {
                // Fetch Dealer with related Borrower and Guarantor
                var dealer = await _dealerRepository.GetByIdAsync(dealerId);
                if (dealer == null)
                    return NotFound("Dealer not found.");

                var uploadedDocuments = new List<DocumentUploadDto>();

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    var documentType = documentTypes[i];

                    if (file == null || file.Length == 0)
                        continue;

                    var filePath = await SaveFileAsync(file);

                    var dto = new DocumentUploadDto
                    {
                        DealerId = dealerId,
                        DocumentType = documentType,
                        FilePath = filePath,
                        FileName = file.FileName
                        // UploadedOn will be set by the service
                    };

                    // Set BorrowerId or GuarantorId if needed based on DocumentType
                    if (IsBorrowerDocument(documentType) && dealer.BorrowerDetails?.Any() == true)
                    {
                        // Uncomment when you have these fields in your DTO
                        // dto.BorrowerDetailsId = dealer.BorrowerDetails.FirstOrDefault().Id;
                    }
                    else if (IsGuarantorDocument(documentType) && dealer.GuarantorDetails?.Any() == true)
                    {
                        // Uncomment when you have these fields in your DTO
                        // dto.GuarantorDetailsId = dealer.GuarantorDetails.FirstOrDefault().Id;
                    }

                    var result = await _documentService.UploadDocumentAsync(dto);
                    uploadedDocuments.Add(result);
                }

                return Ok(uploadedDocuments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading multiple documents");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("upload-multiple-objects")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadMultipleDocumentsAsObjects(
        [FromForm] List<MultipleDocumentUploadDto> documents)
        {
            if (documents == null || documents.Count == 0)
                return BadRequest("No documents provided.");

            try
            {
                var uploadedDocuments = new List<DocumentUploadDto>();

                foreach (var doc in documents)
                {
                    if (doc == null || doc.Document == null || doc.Document.Length == 0 || string.IsNullOrEmpty(doc.DocumentType))
                    {
                        _logger.LogWarning("Skipping document with null or empty fields");
                        continue;
                    }

                    var filePath = await SaveFileAsync(doc.Document);

                    var dto = new DocumentUploadDto
                    {
                        DealerId = doc.DealerId,
                        DocumentType = doc.DocumentType,
                        FilePath = filePath,
                        FileName = doc.Document.FileName
                    };

                    var result = await _documentService.UploadDocumentAsync(dto);
                    uploadedDocuments.Add(result);
                }

                return Ok(uploadedDocuments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading multiple documents as objects");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("upload-multiple-flat")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadMultipleDocumentsFlat(
            [FromForm] List<IFormFile> documents,
            [FromForm] List<string> documentTypes,
            [FromForm] List<int> dealerIds)
        {
            if (documents == null || documents.Count == 0)
                return BadRequest("No documents provided.");

            if (documents.Count != documentTypes.Count || documents.Count != dealerIds.Count)
                return BadRequest("Mismatch between documents, document types, and dealer IDs.");

            try
            {
                var uploadedDocuments = new List<DocumentUploadDto>();

                for (int i = 0; i < documents.Count; i++)
                {
                    if (documents[i] == null || documents[i].Length == 0)
                        continue;

                    var filePath = await SaveFileAsync(documents[i]);

                    var dto = new DocumentUploadDto
                    {
                        DealerId = dealerIds[i],
                        DocumentType = documentTypes[i],
                        FilePath = filePath,
                        FileName = documents[i].FileName
                    };

                    var result = await _documentService.UploadDocumentAsync(dto);
                    uploadedDocuments.Add(result);
                }

                return Ok(uploadedDocuments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading multiple documents with flat structure");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool IsBorrowerDocument(string documentType)
        {
            var borrowerDocs = new List<string>
            {
                "Borrower Address proof",
                "Borrower PAN",
                "Borrower Aadhar Card"
            };
            return borrowerDocs.Contains(documentType);
        }

        private bool IsGuarantorDocument(string documentType)
        {
            var guarantorDocs = new List<string>
            {
                "Guarantor PAN",
                "Guarantor Address Proof",
                "Guarantor Aadhar Card"
            };
            return guarantorDocs.Contains(documentType);
        }

        private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".pdf":
                    return "application/pdf";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".txt":
                    return "text/plain";
                default:
                    return "application/octet-stream";
            }
        }
    }
}