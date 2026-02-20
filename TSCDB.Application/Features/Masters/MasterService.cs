using AutoMapper;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Masters;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Masters
{
    // SOLID (SRP): encapsulates master CRUD use-cases independent of HTTP/persistence details.
    // SOLID (DIP): depends on IGenericRepository<T> and IMapper abstractions.
    // SOLID (OCP): generic design allows adding new master entities without changing service code.
    // Depends on:
    // - TSCDB.Core.Interfaces.Repositories/IGenericRepository.cs
    // - TSCDB.Application/Mappings/MappingProfile.cs
    public class MasterService<T> : IMasterService<T> where T : class, IMasterEntity
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IMapper _mapper;

        public MasterService(IGenericRepository<T> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<MasterDto>> GetAllAsync() =>
            _mapper.Map<List<MasterDto>>(await _repository.GetAllAsync());

        public async Task<MasterDto> GetByIdAsync(int id) =>
            _mapper.Map<MasterDto>(await _repository.GetByIdAsync(id));

        public async Task<MasterDto> CreateAsync(MasterDto dto)
        {
            var entity = _mapper.Map<T>(dto);
            await _repository.AddAsync(entity);
            return _mapper.Map<MasterDto>(entity);
        }

        public async Task<MasterDto> UpdateAsync(int id, MasterDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            entity.Name = dto.Name;
            await _repository.UpdateAsync(entity);
            return _mapper.Map<MasterDto>(entity);
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
