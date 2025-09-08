using AutoMapper;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Loan;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Loans
{
    public class LoanDocumentService : ILoanDocumentService
    {
        private readonly IGenericRepository<LoanDocumentUpload> _repository;
        private readonly IMapper _mapper;

        public LoanDocumentService(IGenericRepository<LoanDocumentUpload> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<LoanDocumentUploadDto> UploadDocumentAsync(LoanDocumentUploadDto dto)
        {
            var entity = _mapper.Map<LoanDocumentUpload>(dto);
            await _repository.InsertAsync(entity);
            return _mapper.Map<LoanDocumentUploadDto>(entity);
        }

        public async Task<List<LoanDocumentUploadDto>> GetDocumentsByLoanIdAsync(int loanId)
        {
            var docs = await _repository.GetAllAsync(x => x.LoanId == loanId);
            return _mapper.Map<List<LoanDocumentUploadDto>>(docs);
        }

        public async Task<LoanDocumentUploadDto> GetDocumentByIdAsync(int id)
        {
            var doc = await _repository.GetByIdAsync(id);
            return _mapper.Map<LoanDocumentUploadDto>(doc);
        }

    }
}
