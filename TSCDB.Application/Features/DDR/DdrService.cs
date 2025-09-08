using AutoMapper;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.DDR;
using TscLoanManagement.TSCDB.Core.Domain.Loan;
using TscLoanManagement.TSCDB.Core.Enums;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TscLoanManagement.TSCDB.Infrastructure.Data.Context;
using DocumentFormat.OpenXml.InkML;


namespace TscLoanManagement.TSCDB.Application.Features
{
    public class DdrService : IDdrService
    {
        private readonly IGenericRepository<DDR> _ddrRepo;
        private readonly IGenericRepository<Loan> _loanRepo;
        private readonly IMapper _mapper;
        private readonly ILoanCalculationService _loanCalculationService;
        private readonly ILoanService _loanService;
        private readonly TSCDbContext _context;
        public DdrService(IGenericRepository<DDR> ddrRepo, IGenericRepository<Loan> loanRepo, IMapper mapper, ILoanCalculationService loanCalculationService, ILoanService loanService, TSCDbContext context)
        {
            _ddrRepo = ddrRepo;
            _loanRepo = loanRepo;
            _mapper = mapper;
            _loanCalculationService = loanCalculationService;
            _loanService = loanService;
            _context = context;
        }

        public async Task<DdrResponseDto> CreateDDRAsync(DdrCreateDto dto)
        {
            if (dto.VehicleInfo != null && !string.IsNullOrWhiteSpace(dto.VehicleInfo.RegistrationNumber))
            {
                var existingDdr = await _ddrRepo.GetFirstOrDefaultAsync(
                    d => d.VehicleInfo != null &&
                         d.VehicleInfo.RegistrationNumber == dto.VehicleInfo.RegistrationNumber
                );

                if (existingDdr != null)
                {
                    //throw new InvalidOperationException($"A DDR with vehicle number '{dto.VehicleInfo.RegistrationNumber}' already exists. in DDR '{existingDdr}'");
                    throw new InvalidOperationException(
                    $"A DDR with vehicle number '{dto.VehicleInfo.RegistrationNumber}' already exists. DDR ID: {existingDdr.Id}, DDR Number: {existingDdr.DDRNumber}"
                );

                }
            }

            var ddr = _mapper.Map<DDR>(dto);
            ddr.DDRNumber = await GenerateDdrNumberAsync();
            ddr.Status = DDRStatus.PendingApproval;
            ddr.CreatedDate = DateTime.UtcNow;
            ddr.IsActive = true;

            if (dto.Amount.HasValue)
            {
                ddr.SuggestedProcessingFee = CalculateSuggestedProcessingFee(dto.Amount.Value);
                ddr.SelectedProcessingFee = dto.SelectedProcessingFee ?? ddr.SuggestedProcessingFee;
            }

            await _ddrRepo.InsertAsync(ddr);
            var createdDdr = await _ddrRepo.GetByIdAsync(ddr.Id);
            return _mapper.Map<DdrResponseDto>(createdDdr);
        }



        public async Task<DdrResponseDto> UpdateDDRAsync(int id, DdrUpdateDto dto)
        {
            var stopwatch = Stopwatch.StartNew();

            var existingDdr = await _ddrRepo.GetFirstOrDefaultAsync(
                d => d.Id == id,
                include: d => d.Include(x => x.VehicleInfo)
            );

            stopwatch.Stop();
            Console.WriteLine($"Fetch took {stopwatch.ElapsedMilliseconds} ms");

            if (existingDdr == null)
                throw new KeyNotFoundException($"DDR with ID {id} not found");

            // 🚫 Check for duplicate vehicle number
            if (dto.VehicleInfo != null && !string.IsNullOrWhiteSpace(dto.VehicleInfo.RegistrationNumber))
            {
                var duplicateDdr = await _ddrRepo.GetFirstOrDefaultAsync(
                    d => d.Id != id &&
                         d.VehicleInfo != null &&
                         d.VehicleInfo.RegistrationNumber == dto.VehicleInfo.RegistrationNumber
                );

                if (duplicateDdr != null)
                {
                    throw new InvalidOperationException($"Another DDR with vehicle number '{dto.VehicleInfo.RegistrationNumber}' already exists.");
                }
            }

            _mapper.Map(dto, existingDdr); // Map flat properties

            if (dto.Amount.HasValue)
            {
                existingDdr.SuggestedProcessingFee = CalculateSuggestedProcessingFee(dto.Amount.Value);

                if (dto.SelectedProcessingFee.HasValue)
                {
                    existingDdr.SelectedProcessingFee = dto.SelectedProcessingFee;
                    existingDdr.ProcessingFee = dto.SelectedProcessingFee.Value;
                    existingDdr.GSTOnProcessingFee = 18;
                }
                else
                {
                    existingDdr.SelectedProcessingFee = null;
                    existingDdr.ProcessingFee = null;
                    existingDdr.GSTOnProcessingFee = null;
                }
            }

            // 🚘 Map VehicleInfo
            if (dto.VehicleInfo != null)
            {
                if (existingDdr.VehicleInfo == null)
                {
                    existingDdr.VehicleInfo = _mapper.Map<VehicleInfo>(dto.VehicleInfo);
                }
                else
                {
                    _mapper.Map(dto.VehicleInfo, existingDdr.VehicleInfo);
                    _context.Entry(existingDdr.VehicleInfo).State = EntityState.Modified;
                }
            }

            await _ddrRepo.UpdateAsync(existingDdr);
            return _mapper.Map<DdrResponseDto>(existingDdr);
        }




        public async Task<DdrResponseDto> GetDDRByIdAsync(int id)
        {
            var ddr = await _ddrRepo.GetFirstOrDefaultAsync(
             d => d.Id == id,
             include: d => d.Include(x => x.Dealer)
                            .Include(x => x.VehicleInfo)
         );

            if (ddr == null)
                throw new KeyNotFoundException($"DDR with ID {id} not found");

            return _mapper.Map<DdrResponseDto>(ddr);
        }

        public async Task<List<DdrResponseDto>> GetAllDDRsAsync()
        {
            var ddrs = await _ddrRepo.GetAllAsync(include: d =>
        d.Include(x => x.Dealer)
         .Include(x => x.VehicleInfo)
    );

            return _mapper.Map<List<DdrResponseDto>>(ddrs);
        }

        public async Task<List<DdrResponseDto>> GetDDRsByDealerAsync(int dealerId)
        {
            var ddrs = await _ddrRepo.GetAllAsync(d => d.DealerId == dealerId);
            return _mapper.Map<List<DdrResponseDto>>(ddrs);
        }

        public async Task<List<DdrResponseDto>> GetDDRsByStatusAsync(DDRStatus status)
        {
            var ddrs = await _ddrRepo.GetAllAsync(d => d.Status == status);
            return _mapper.Map<List<DdrResponseDto>>(ddrs);
        }

        public async Task ApproveDDRAsync(int ddrId)
        {

            var ddr = await _ddrRepo.GetByIdAsync(ddrId);
       
           
            if (ddr == null)
                throw new KeyNotFoundException($"DDR with ID {ddrId} not found");

            //if (ddr.Status != DDRStatus.PendingDisbursement)
            //    throw new InvalidOperationException("Only pending DDRs can be approved");

            // ✅ Map DDR to Loan
            var loan = _mapper.Map<Loan>(ddr);
            loan.DDRId = ddr.Id;

            loan.CreatedDate = DateTime.UtcNow;
            loan.IsActive = true;

            // ✅ LoanNumber generation (reusing DDR number OR generate new one)
            loan.LoanNumber = await _loanService.GenerateLoanNumberAsync(); // or ;

            // ✅ Insert Loan
            await _loanRepo.InsertAsync(loan);

            // ✅ Create LoanDetail from LoanCalculationService
            try
            {
                var createLoanRequest = new CreateLoanRequestDto
                {
                    CustomerId = loan.DealerId,
                    PrincipalAmount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    ProcessingFeeRate = loan.ProcessingFee,
                    GSTPercent = loan.GSTOnProcessingFee ?? 0,
                    StartDate = loan.DateOfWithdraw,
                    DueDays = 60,
                    DelayInterestRate = loan.DelayedROI ?? 0
                };

                var loanDetail = await _loanCalculationService.CreateLoanAsync(createLoanRequest);
                loan.LoanDetailId = loanDetail.LoanId;

                await _loanRepo.UpdateAsync(loan);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoanDetail creation failed for DDR {ddr.Id}: {ex.Message}");
                // Optionally mark the loan or DDR as failed
            }

            // ✅ Mark DDR as Approved
            ddr.Status = DDRStatus.Processed;
            ddr.ApprovalDate = DateTime.UtcNow;
            await _ddrRepo.UpdateAsync(ddr);
        }


        public async Task RejectDDRAsync(int ddrId, string rejectedBy, string reason)
        {
            var ddr = await _ddrRepo.GetByIdAsync(ddrId);
            if (ddr == null)
                throw new KeyNotFoundException($"DDR with ID {ddrId} not found");

            if (ddr.Status == DDRStatus.Approved)
                throw new InvalidOperationException("Cannot reject approved DDR");

            ddr.Status = DDRStatus.Rejected;
            ddr.RejectedBy = rejectedBy;
            ddr.RejectedReason = reason;
            ddr.RejectionDate = DateTime.UtcNow;
            await _ddrRepo.UpdateAsync(ddr);
        }

        public async Task HoldDDRAsync(int ddrId, string holdBy, string reason)
        {
            var ddr = await _ddrRepo.GetByIdAsync(ddrId);
            if (ddr == null)
                throw new KeyNotFoundException($"DDR with ID {ddrId} not found");

            if (ddr.Status == DDRStatus.Approved)
                throw new InvalidOperationException("Cannot hold approved DDR");

            ddr.Status = DDRStatus.Hold;
            ddr.HoldBy = holdBy;
            ddr.HoldReason = reason;
            ddr.HoldDate = DateTime.UtcNow;
            await _ddrRepo.UpdateAsync(ddr);
        }

        public async Task<bool> UpdateDdrStatusAsync(DdrStatusUpdateDto dto)
{
    var ddr = await _ddrRepo.GetByIdAsync(dto.DdrId);
    if (ddr == null) return false;

    ddr.Status = dto.Status;
    await _ddrRepo.UpdateAsync(ddr); // assumes UpdateAsync handles SaveChanges
    return true;
}


        public async Task DeleteDDRAsync(int id)
        {
            var ddr = await _ddrRepo.GetByIdAsync(id);
            if (ddr == null)
                throw new KeyNotFoundException($"DDR with ID {id} not found");

            if (ddr.Status == DDRStatus.Approved)
                throw new InvalidOperationException("Cannot delete approved DDR");

            await _ddrRepo.DeleteAsync(ddr);
        }
        public async Task<string> GenerateDdrNumberAsync()
        {
            var latestDdr = await _ddrRepo
                .GetQueryable()
                .OrderByDescending(d => d.DDRNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (latestDdr != null && !string.IsNullOrEmpty(latestDdr.DDRNumber))
            {
                var numericPart = new string(latestDdr.DDRNumber
                    .SkipWhile(c => !char.IsDigit(c))
                    .ToArray());

                if (int.TryParse(numericPart, out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            return $"TSC-DDR{nextNumber:D3}";
        }

        private decimal CalculateSuggestedProcessingFee(decimal principalAmount)
        {
            const decimal slabAmount = 50000m;
            const decimal slabFee = 100m;
            const decimal tolerancePercent = 0.20m;

            int fullSlabs = (int)(principalAmount / slabAmount);
            decimal remainder = principalAmount % slabAmount;
            decimal toleranceAmount = slabAmount * tolerancePercent;

            if (remainder <= toleranceAmount)
                return fullSlabs * slabFee;
            else
                return (fullSlabs + 1) * slabFee;
        }



    }

}
