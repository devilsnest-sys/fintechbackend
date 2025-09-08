using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DDRDocumentUploadController : ControllerBase
    {
        private readonly IDDRDocumentService _ddrDocumentService;
        private readonly ILogger<DDRDocumentUploadController> _logger;
        private readonly IWebHostEnvironment _env;

        public DDRDocumentUploadController(
            IDDRDocumentService ddrDocumentService,
            ILogger<DDRDocumentUploadController> logger,
            IWebHostEnvironment env)
        {
            _ddrDocumentService = ddrDocumentService;
            _logger = logger;
            _env = env;
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var folderPath = Path.Combine("Uploads", "DDRDocuments");
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

        //[HttpPost("upload-multiple-documents")]
        //public async Task<IActionResult> UploadMultipleDocuments([FromForm] List<DDRDocumentUploadDto> documents)
        //{
        //    if (documents == null || documents.Count == 0)
        //        return BadRequest("No documents provided.");

        //    var uploadedDocs = new List<DDRDocumentUploadDto>();

        //    foreach (var doc in documents)
        //    {
        //        if (doc?.Document == null || string.IsNullOrEmpty(doc.DocumentType))
        //            continue;

        //        var path = await SaveFileAsync(doc.Document);
        //        var dto = new DDRDocumentUploadDto
        //        {
        //            DdrId = doc.DdrId,
        //            DocumentType = doc.DocumentType,
        //            FileName = doc.Document.FileName,
        //            FilePath = path,
        //            UploadedOn = DateTime.UtcNow
        //        };

        //        var result = await _ddrDocumentService.UploadDocumentAsync(dto);
        //        uploadedDocs.Add(result);
        //    }

        //    return Ok(uploadedDocs);
        //}

        [HttpPost("upload-multiple-documents")]
        public async Task<IActionResult> UploadMultipleDocuments([FromForm] List<DDRDocumentUploadDto> documents)
        {
            if (documents == null || documents.Count == 0)
                return BadRequest("No documents provided.");

            var uploadedDocs = new List<DDRDocumentUploadDto>();

            foreach (var doc in documents)
            {
                if (doc?.Document == null || string.IsNullOrEmpty(doc.DocumentType))
                    continue;

                // Fetch all existing docs for the DDR ID
                var existingDocs = await _ddrDocumentService.GetDocumentsByDdrIdAsync(doc.DdrId);

                // Match based on DocumentType instead of FileName
                var matchingDoc = existingDocs
                    .FirstOrDefault(d => string.Equals(d.DocumentType, doc.DocumentType, StringComparison.OrdinalIgnoreCase));

                // If matching document exists, delete the old file from disk
                if (matchingDoc != null)
                {
                    var existingPath = Path.Combine(
                        _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                        matchingDoc.FilePath?.Replace("http://localhost:5029/", "").Replace("/", Path.DirectorySeparatorChar.ToString()) ?? ""
                    );

                    if (System.IO.File.Exists(existingPath))
                        System.IO.File.Delete(existingPath);
                }

                // Save new file to disk
                var path = await SaveFileAsync(doc.Document);

                // Prepare DTO for insert/update
                var dto = new DDRDocumentUploadDto
                {
                    DdrId = doc.DdrId,
                    DocumentType = doc.DocumentType,
                    FileName = doc.Document.FileName,
                    FilePath = path,
                    UploadedOn = DateTime.UtcNow
                };

                if (matchingDoc != null)
                {
                    dto.Id = matchingDoc.Id; // Pass the existing ID to trigger an update in your service logic
                }

                var result = await _ddrDocumentService.UploadDocumentAsync(dto); // insert or update logic inside service
                uploadedDocs.Add(result);
            }

            return Ok(uploadedDocs);
        }



        [HttpGet("by-ddr/{ddrId}")]
        public async Task<IActionResult> GetDocumentsByDdrId(int ddrId)
        {
            var documents = await _ddrDocumentService.GetDocumentsByDdrIdAsync(ddrId);
            if (documents == null || documents.Count == 0)
                return Ok(documents);
            //return Ok($"No documents found for DDR ID {ddrId}");

            foreach (var doc in documents)
            {
                if (!string.IsNullOrEmpty(doc.FilePath))
                    doc.FilePath = $"{Request.Scheme}://{Request.Host}/{doc.FilePath}";
            }

            return Ok(documents);
        }

        [HttpGet("view/{id}")]
        public async Task<IActionResult> ViewDocument(int id)
        {
            var doc = await _ddrDocumentService.GetDocumentByIdAsync(id);
            if (doc == null)
                return NotFound($"Document with ID {id} not found");

            return Ok(doc);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var doc = await _ddrDocumentService.GetDocumentByIdAsync(id);
            if (doc == null)
                return NotFound($"Document with ID {id} not found");

            var fullPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), doc.FilePath);
            if (!System.IO.File.Exists(fullPath))
                return NotFound($"File not found on server for Document ID {id}");

            var contentType = "application/octet-stream";
            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            return File(fileBytes, contentType, doc.FileName);
        }
    }

}
