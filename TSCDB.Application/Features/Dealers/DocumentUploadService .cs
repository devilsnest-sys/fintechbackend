using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Dealer;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Dealers
{
    public class DocumentUploadService : IDocumentUploadService
    {
        private readonly IGenericRepository<DocumentUpload> _documentRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<DocumentUploadService> _logger;
        private readonly string _webRootPath;

        public DocumentUploadService(
            IGenericRepository<DocumentUpload> documentRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment,
            ILogger<DocumentUploadService> logger)
        {
            _documentRepo = documentRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;

            // Store WebRootPath and log if it's null
            _webRootPath = _webHostEnvironment?.WebRootPath;
            if (_webRootPath == null)
            {
                _logger.LogWarning("WebRootPath is null in DocumentUploadService constructor");
            }
        }

        public async Task<DocumentUploadDto> UploadDocumentAsync(DocumentUploadDto dto)
        {
            try
            {
                if (!string.IsNullOrEmpty(dto.FilePath))
                {
                    dto.FilePath = NormalizePath(dto.FilePath);
                }

                // Check if document already exists (DealerId + DocumentType)
                var existing = await _documentRepo.GetFirstOrDefaultAsync(d =>
                    d.DealerId == dto.DealerId &&
                    d.DocumentType.ToLower() == dto.DocumentType.ToLower());

                if (existing != null)
                {
                    // Delete old file from disk
                    var fullOldPath = Path.Combine(
                        _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                        existing.FilePath?.TrimStart('/') ?? "");

                    if (System.IO.File.Exists(fullOldPath))
                        System.IO.File.Delete(fullOldPath);

                    // Update existing entity
                    existing.FilePath = dto.FilePath;
                    existing.DocumentType = dto.DocumentType;
                    existing.UploadedOn = DateTime.UtcNow;

                    await _documentRepo.UpdateAsync(existing);
                    return _mapper.Map<DocumentUploadDto>(existing);
                }
                else
                {
                    // Insert new
                    var entity = _mapper.Map<DocumentUpload>(dto);
                    entity.UploadedOn = DateTime.UtcNow;

                    await _documentRepo.AddAsync(entity);
                    return _mapper.Map<DocumentUploadDto>(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                throw;
            }
        }


        public async Task<DocumentUploadDto> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _documentRepo.GetByIdAsync(id);
                if (entity == null)
                {
                    return null;
                }

                var dto = _mapper.Map<DocumentUploadDto>(entity);

                // Important: Don't convert to full URL here - let the controller handle that
                // This ensures we can properly check file existence at the physical path

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting document with ID {id}");
                throw;
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty.");

            // Ensure we have a valid web root path
            var basePath = !string.IsNullOrEmpty(_webRootPath)
                ? _webRootPath
                : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var uploadsDir = Path.Combine(basePath, "uploads");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{fileName}";
        }

        public async Task<List<DocumentUploadDto>> GetByDealerIdAsync(int dealerId)
        {
            try
            {
                var entities = await _documentRepo.GetAllAsync(d => d.DealerId == dealerId);
                var dtos = _mapper.Map<List<DocumentUploadDto>>(entities);

                // Important: Don't convert to full URLs here - let the controller handle that

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting documents for dealer ID {dealerId}");
                throw;
            }
        }

        // Helper methods
        private string NormalizePath(string path)
        {
            // Ensure path starts with /
            if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
            {
                path = "/" + path;
            }

            return path;
        }

        private bool FileExists(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return false;

            // Remove leading / for combining with web root path
            relativePath = relativePath.TrimStart('/');

            // Ensure we have a valid web root path
            var basePath = !string.IsNullOrEmpty(_webRootPath)
                ? _webRootPath
                : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            string fullPath = Path.Combine(basePath, relativePath);
            return File.Exists(fullPath);
        }
    }
}