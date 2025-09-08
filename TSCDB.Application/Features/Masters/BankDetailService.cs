using AutoMapper;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Masters;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Masters
{
    public class BankDetailService : IBankDetailService
    {
        private readonly IGenericRepository<BankDetail> _repository;
        private readonly IMapper _mapper;

        public BankDetailService(IGenericRepository<BankDetail> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<BankDetailDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<List<BankDetailDto>>(entities);
        }

        public async Task<BankDetailDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<BankDetailDto>(entity);
        }

        public async Task<BankDetailDto> CreateAsync(BankDetailDto dto)
        {
            var entity = _mapper.Map<BankDetail>(dto);
            await _repository.AddAsync(entity);
            return _mapper.Map<BankDetailDto>(entity);
        }

        public async Task<BankDetailDto> UpdateAsync(int id, BankDetailDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);
            return _mapper.Map<BankDetailDto>(entity);
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
