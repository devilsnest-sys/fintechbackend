using AutoMapper;
using TscLoanManagement.TSCDB.Application.DTOs.PurchaseSourceDocuments;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Masters;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Masters
{
    public class PurchaseSourceDocumentService : IPurchaseSourceDocumentService
    {
        private readonly IGenericRepository<PurchaseSourceDocument1> _repository;
        private readonly IMapper _mapper;

        public PurchaseSourceDocumentService(
            IGenericRepository<PurchaseSourceDocument1> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PurchaseSourceDocumentDto>> GetByPurchaseSourceIdAsync(int sourceId)
        {
            var docs = await _repository.GetAllAsync(x => x.PurchaseSourceId == sourceId);
            return _mapper.Map<IEnumerable<PurchaseSourceDocumentDto>>(docs);
        }

        public async Task<IEnumerable<PurchaseSourceDocumentDto>> CreateManyAsync(PurchaseSourceDocumentCreateManyDto dto)
        {
            // Fetch existing documents for the given PurchaseSourceId
            var existingDocs = await _repository.GetAllAsync(x => x.PurchaseSourceId == dto.PurchaseSourceId);

            var resultList = new List<PurchaseSourceDocument1>();

            foreach (var docName in dto.DocumentNames)
            {
                var existingDoc = existingDocs.FirstOrDefault(x =>
                    x.DocumentName.Trim().ToLower() == docName.Trim().ToLower());

                if (existingDoc != null)
                {
                    // Update the existing record
                    existingDoc.is_mandatory = dto.is_mandatory;
                    await _repository.UpdateAsync(existingDoc);
                    resultList.Add(existingDoc);
                }
                else
                {
                    // Create a new record
                    var newDoc = new PurchaseSourceDocument1
                    {
                        PurchaseSourceId = dto.PurchaseSourceId,
                        DocumentName = docName,
                        is_mandatory = dto.is_mandatory
                    };
                    await _repository.AddAsync(newDoc);
                    resultList.Add(newDoc);
                }
            }

            return _mapper.Map<IEnumerable<PurchaseSourceDocumentDto>>(resultList);
        }

    }

}
