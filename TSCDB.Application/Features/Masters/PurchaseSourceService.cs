using AutoMapper;
using TscLoanManagement.TSCDB.Application.DTOs.PurchaseSourceDocuments;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Masters;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Masters
{
    public class PurchaseSourceService : IPurchaseSourceService
    {
        private readonly IGenericRepository<PurchaseSource> _repository;
        private readonly IMapper _mapper;

        public PurchaseSourceService(IGenericRepository<PurchaseSource> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PurchaseSourceDto>> GetAllAsync() =>
            _mapper.Map<List<PurchaseSourceDto>>(await _repository.GetAllAsync());

        public async Task<PurchaseSourceDto> GetByIdAsync(int id) =>
            _mapper.Map<PurchaseSourceDto>(await _repository.GetByIdAsync(id));

        public async Task<PurchaseSourceDto> CreateAsync(PurchaseSourceDto dto)
        {
            var entity = _mapper.Map<PurchaseSource>(dto);
            await _repository.AddAsync(entity);
            return _mapper.Map<PurchaseSourceDto>(entity);
        }

        public async Task<PurchaseSourceDto> UpdateAsync(int id, PurchaseSourceDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Name = dto.Name;
            entity.LoanBidAmtPercentage = dto.LoanBidAmtPercentage;
            entity.IsApplicationNoRequired = dto.IsApplicationNoRequired;

            await _repository.UpdateAsync(entity);
            return _mapper.Map<PurchaseSourceDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            await _repository.DeleteAsync(entity);
            return true;
        }
    }

}
