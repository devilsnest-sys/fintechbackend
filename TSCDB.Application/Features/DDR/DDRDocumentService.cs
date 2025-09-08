using AutoMapper;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.DDR;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;
using TscLoanManagement.TSCDB.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace TscLoanManagement.TSCDB.Application.Features
{
    public class DDRDocumentService : IDDRDocumentService
    {
        private readonly IGenericRepository<DDRDocument> _repository;
        private readonly IMapper _mapper;
        private readonly TSCDbContext _context;

        public DDRDocumentService(IGenericRepository<DDRDocument> repository, IMapper mapper, TSCDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<DDRDocumentUploadDto> UploadDocumentAsync(DDRDocumentUploadDto dto)
        {
            // Try to find existing document by DDR ID + Document Type
            var existing = await _context.DDRDocuments
                .FirstOrDefaultAsync(d =>
                    d.DdrId == dto.DdrId &&
                    d.DocumentType.ToLower() == dto.DocumentType.ToLower());

            if (existing != null)
            {
                // Update directly via DbContext
                existing.FileName = dto.FileName;
                existing.FilePath = dto.FilePath;
                existing.UploadedOn = dto.UploadedOn;

                _context.DDRDocuments.Update(existing);
                await _context.SaveChangesAsync();

                dto.Id = existing.Id; // Return updated ID
            }
            else
            {
                // Insert new document via generic repo
                var entity = _mapper.Map<DDRDocument>(dto);
                await _repository.InsertAsync(entity);
                dto.Id = entity.Id;
            }

            return dto;
        }


        public async Task<List<DDRDocumentUploadDto>> GetDocumentsByDdrIdAsync(int ddrId)
        {
            var docs = await _repository.GetAllAsync(x => x.DdrId == ddrId);
            return _mapper.Map<List<DDRDocumentUploadDto>>(docs);
        }

        public async Task<DDRDocumentUploadDto?> GetDocumentByIdAsync(int id)
        {
            var doc = await _repository.GetByIdAsync(id);
            return _mapper.Map<DDRDocumentUploadDto>(doc);
        }
    }
}
