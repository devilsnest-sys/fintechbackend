using AutoMapper;
using System.Text.Json;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Application.Wrappers;
using TscLoanManagement.TSCDB.Core.Domain.Dealer;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;
using TscLoanManagement.TSCDB.Infrastructure.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Dealers
{
    public class DealerDetailsService : IDealerDetailsService
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<BorrowerDetails> _borrowerRepo;
        private readonly IGenericRepository<GuarantorDetails> _guarantorRepo;
        private readonly IGenericRepository<ChequeDetails> _chequeRepo;
        private readonly IGenericRepository<SecurityDepositDetails> _securityRepo;
        private readonly IDealerRepository _dealerRepository;

        public DealerDetailsService(
            IMapper mapper,
            IGenericRepository<BorrowerDetails> borrowerRepo,
            IGenericRepository<GuarantorDetails> guarantorRepo,
            IGenericRepository<ChequeDetails> chequeRepo,
            IGenericRepository<SecurityDepositDetails> securityRepo,
            IDealerRepository dealerRepository)
        {
            _mapper = mapper;
            _borrowerRepo = borrowerRepo;
            _guarantorRepo = guarantorRepo;
            _chequeRepo = chequeRepo;
            _securityRepo = securityRepo;
            _dealerRepository = dealerRepository;
        }

        #region Create Operations

        public async Task<BorrowerDetailsDto> SubmitBorrowerDetailsAsync(BorrowerDetailsDto dto)
        {
            var entity = _mapper.Map<BorrowerDetails>(dto);
            await _borrowerRepo.AddAsync(entity);
            return _mapper.Map<BorrowerDetailsDto>(entity);
        }

        public async Task<GuarantorDetailsDto> SubmitGuarantorDetailsAsync(GuarantorDetailsDto dto)
        {
            var entity = _mapper.Map<GuarantorDetails>(dto);
            await _guarantorRepo.AddAsync(entity);
            return _mapper.Map<GuarantorDetailsDto>(entity);
        }

        public async Task<ChequeDetailsDto> SubmitChequeDetailsAsync(ChequeDetailsDto dto)
        {
            var entity = _mapper.Map<ChequeDetails>(dto);
            await _chequeRepo.AddAsync(entity);
            return _mapper.Map<ChequeDetailsDto>(entity);
        }

        public async Task<SecurityDepositDetailsDto> SubmitSecurityDepositDetailsAsync(SecurityDepositDetailsDto dto)
        {
            var entity = _mapper.Map<SecurityDepositDetails>(dto);
            await _securityRepo.AddAsync(entity);

            // Sync with Dealer table
            var dealer = await _dealerRepository.GetByIdAsync(dto.DealerId);
            if (dealer != null)
            {
                dealer.CIBILOfEntity = dto.CIBILOfEntity;
                dealer.TotalSanctionLimit = dto.TotalSanctionLimit;
                dealer.ROI = dto.ROI;
                dealer.ROIPerLakh = dto.ROIPerLakh;
                dealer.DelayROI = dto.DelayROI;
                dealer.ProcessingFee = dto.ProcessingFee;
                dealer.ProcessingCharge = dto.ProcessingCharge;
                dealer.GSTOnProcessingCharge = dto.GSTOnProcessingCharge;
                dealer.DocumentationCharge = dto.DocumentationCharge;
                dealer.GSTOnDocCharges = dto.GSTOnDocCharges;
                dealer.RejectionReason = dto.RejectionReason;
                dealer.IsActive = dto.IsActive;
                dealer.AgreementDate = dto.AgreementDate;

                await _dealerRepository.UpdateAsync(dealer);
            }

            return _mapper.Map<SecurityDepositDetailsDto>(entity);
        }


        public async Task<ApiResponse<string>> SubmitFullDealerDetailsAsync(DealerFullDetailsDto dto)
        {
            try
            {
                var borrowerEntities = _mapper.Map<List<BorrowerDetails>>(dto.BorrowerDetails);
                var guarantorEntities = _mapper.Map<List<GuarantorDetails>>(dto.GuarantorDetails);
                var cheque = _mapper.Map<ChequeDetails>(dto.ChequeDetails);
                var security = _mapper.Map<SecurityDepositDetails>(dto.SecurityDepositDetails);

                foreach (var borrower in borrowerEntities)
                {
                    await _borrowerRepo.AddAsync(borrower);
                }

                foreach (var guarantor in guarantorEntities)
                {
                    await _guarantorRepo.AddAsync(guarantor);
                }

                await _chequeRepo.AddAsync(cheque);
                await _securityRepo.AddAsync(security);

                return new ApiResponse<string> { Success = true, Message = "Dealer details saved successfully." };
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return new ApiResponse<string> { Success = false, Message = $"Error: {message}" };
            }
        }

        #endregion

        #region Update Operations

        public async Task<BorrowerDetailsDto> UpdateBorrowerDetailsAsync(BorrowerDetailsDto dto)
        {
            var existingEntity = await _borrowerRepo.GetByIdAsync(dto.Id);
            if (existingEntity == null)
                return null;

            // Map updated fields from DTO to entity
            _mapper.Map(dto, existingEntity);

            await _borrowerRepo.UpdateAsync(existingEntity);
            return _mapper.Map<BorrowerDetailsDto>(existingEntity);
        }

        public async Task<GuarantorDetailsDto> UpdateGuarantorDetailsAsync(GuarantorDetailsDto dto)
        {
            var existingEntity = await _guarantorRepo.GetByIdAsync(dto.Id);
            if (existingEntity == null)
                return null;

            // Map updated fields from DTO to entity
            _mapper.Map(dto, existingEntity);

            await _guarantorRepo.UpdateAsync(existingEntity);
            return _mapper.Map<GuarantorDetailsDto>(existingEntity);
        }

        public async Task<ChequeDetailsDto> UpdateChequeDetailsAsync(ChequeDetailsDto dto)
        {
            var existingEntity = await _chequeRepo.GetByIdAsync(dto.Id);
            if (existingEntity == null)
                return null;

            // Map updated fields from DTO to entity
            _mapper.Map(dto, existingEntity);

            await _chequeRepo.UpdateAsync(existingEntity);
            return _mapper.Map<ChequeDetailsDto>(existingEntity);
        }

        public async Task<SecurityDepositDetailsDto> UpdateSecurityDepositDetailsAsync(SecurityDepositDetailsDto dto)
        {
            var existingEntity = await _securityRepo.GetByIdAsync(dto.Id);
            if (existingEntity == null)
                return null;

            _mapper.Map(dto, existingEntity);
            await _securityRepo.UpdateAsync(existingEntity);

            // Sync with Dealer table
            var dealer = await _dealerRepository.GetByIdAsync(dto.DealerId);
            if (dealer != null)
            {
                dealer.CIBILOfEntity = dto.CIBILOfEntity;
                dealer.TotalSanctionLimit = dto.TotalSanctionLimit;
                dealer.ROI = dto.ROI;
                dealer.ROIPerLakh = dto.ROIPerLakh;
                dealer.DelayROI = dto.DelayROI;
                dealer.ProcessingFee = dto.ProcessingFee;
                dealer.ProcessingCharge = dto.ProcessingCharge;
                dealer.GSTOnProcessingCharge = dto.GSTOnProcessingCharge;
                dealer.DocumentationCharge = dto.DocumentationCharge;
                dealer.GSTOnDocCharges = dto.GSTOnDocCharges;
                dealer.RejectionReason = dto.RejectionReason;
                dealer.AgreementDate = dto.AgreementDate;
                dealer.IsActive = dto.IsActive;

                await _dealerRepository.UpdateAsync(dealer);
            }

            return _mapper.Map<SecurityDepositDetailsDto>(existingEntity);
        }


        public async Task<ApiResponse<string>> UpdateFullDealerDetailsAsync(DealerFullDetailsDto dto)
        {
            try
            {
                // Update borrower details
                if (dto.BorrowerDetails != null && dto.BorrowerDetails.Any())
                {
                    foreach (var borrowerDto in dto.BorrowerDetails)
                    {
                        if (borrowerDto.Id > 0)
                        {
                            var existingBorrower = await _borrowerRepo.GetByIdAsync(borrowerDto.Id);
                            if (existingBorrower != null)
                            {
                                _mapper.Map(borrowerDto, existingBorrower);
                                await _borrowerRepo.UpdateAsync(existingBorrower);
                            }
                            else
                            {
                                // Handle not found - create new if ID doesn't exist
                                var newBorrower = _mapper.Map<BorrowerDetails>(borrowerDto);
                                await _borrowerRepo.AddAsync(newBorrower);
                            }
                        }
                        else
                        {
                            // New borrower - add if ID is 0 or not provided
                            var newBorrower = _mapper.Map<BorrowerDetails>(borrowerDto);
                            await _borrowerRepo.AddAsync(newBorrower);
                        }
                    }
                }

                // Update guarantor details
                if (dto.GuarantorDetails != null && dto.GuarantorDetails.Any())
                {
                    foreach (var guarantorDto in dto.GuarantorDetails)
                    {
                        if (guarantorDto.Id > 0)
                        {
                            var existingGuarantor = await _guarantorRepo.GetByIdAsync(guarantorDto.Id);
                            if (existingGuarantor != null)
                            {
                                _mapper.Map(guarantorDto, existingGuarantor);
                                await _guarantorRepo.UpdateAsync(existingGuarantor);
                            }
                            else
                            {
                                // Handle not found - create new if ID doesn't exist
                                var newGuarantor = _mapper.Map<GuarantorDetails>(guarantorDto);
                                await _guarantorRepo.AddAsync(newGuarantor);
                            }
                        }
                        else
                        {
                            // New guarantor - add if ID is 0 or not provided
                            var newGuarantor = _mapper.Map<GuarantorDetails>(guarantorDto);
                            await _guarantorRepo.AddAsync(newGuarantor);
                        }
                    }
                }

                // Update cheque details
                if (dto.ChequeDetails != null && dto.ChequeDetails.Id > 0)
                {
                    var existingCheque = await _chequeRepo.GetByIdAsync(dto.ChequeDetails.Id);
                    if (existingCheque != null)
                    {
                        _mapper.Map(dto.ChequeDetails, existingCheque);
                        await _chequeRepo.UpdateAsync(existingCheque);
                    }
                    else
                    {
                        // Handle not found - create new if ID doesn't exist
                        var newCheque = _mapper.Map<ChequeDetails>(dto.ChequeDetails);
                        await _chequeRepo.AddAsync(newCheque);
                    }
                }
                else if (dto.ChequeDetails != null)
                {
                    // New cheque details - add if ID is 0 or not provided
                    var newCheque = _mapper.Map<ChequeDetails>(dto.ChequeDetails);
                    await _chequeRepo.AddAsync(newCheque);
                }

                // Update security deposit details
                if (dto.SecurityDepositDetails != null && dto.SecurityDepositDetails.Id > 0)
                {
                    var existingSecurity = await _securityRepo.GetByIdAsync(dto.SecurityDepositDetails.Id);
                    if (existingSecurity != null)
                    {
                        _mapper.Map(dto.SecurityDepositDetails, existingSecurity);
                        await _securityRepo.UpdateAsync(existingSecurity);
                    }
                    else
                    {
                        // Handle not found - create new if ID doesn't exist
                        var newSecurity = _mapper.Map<SecurityDepositDetails>(dto.SecurityDepositDetails);
                        await _securityRepo.AddAsync(newSecurity);
                    }
                }
                else if (dto.SecurityDepositDetails != null)
                {
                    // New security deposit details - add if ID is 0 or not provided
                    var newSecurity = _mapper.Map<SecurityDepositDetails>(dto.SecurityDepositDetails);
                    await _securityRepo.AddAsync(newSecurity);
                }

                return new ApiResponse<string> { Success = true, Message = "Dealer details updated successfully." };
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return new ApiResponse<string> { Success = false, Message = $"Error: {message}" };
            }
        }

        public async Task<ApiResponse<string>> SaveBorrowerAndGuarantorDetailsAsync(BorrowerAndGuarantorDetailsDto dto)
        {
            try
            {
                // 🔒 Load the dealer once using DealerId from any borrower or guarantor
                int dealerId = dto.Guarantors?.FirstOrDefault()?.DealerId ?? dto.Borrowers?.FirstOrDefault()?.DealerId ?? 0;
                if (dealerId == 0)
                    return new ApiResponse<string> { Success = false, Message = "DealerId is missing in request." };

                var dealer = await _dealerRepository.GetByIdAsync(dealerId);
                if (dealer == null)
                    return new ApiResponse<string> { Success = false, Message = "Dealer not found." };

                // ✅ Save/Update Borrowers
                if (dto.Borrowers != null && dto.Borrowers.Any())
                {
                    foreach (var borrowerDto in dto.Borrowers)
                    {
                        var borrower = _mapper.Map<BorrowerDetails>(borrowerDto);

                        if (borrowerDto.Id > 0)
                        {
                            var existingBorrower = await _borrowerRepo.GetByIdAsync(borrowerDto.Id);
                            if (existingBorrower != null)
                            {
                                _mapper.Map(borrowerDto, existingBorrower);
                                await _borrowerRepo.UpdateAsync(existingBorrower);
                            }
                            else
                            {
                                await _borrowerRepo.AddAsync(borrower);
                            }
                        }
                        else
                        {
                            await _borrowerRepo.AddAsync(borrower);
                        }
                    }
                }

                // ✅ Save/Update Guarantors with email check
                if (dto.Guarantors != null && dto.Guarantors.Any())
                {
                    foreach (var guarantorDto in dto.Guarantors)
                    {
                        if (!string.IsNullOrWhiteSpace(guarantorDto.Email) &&
                            guarantorDto.Email.Trim().ToLower() == dealer.EmailId?.Trim().ToLower())
                        {
                            return new ApiResponse<string>
                            {
                                Success = false,
                                Message = $"Guarantor email '{guarantorDto.Email}' cannot be same as Dealer email '{dealer.EmailId}'"
                            };
                        }

                        if (!string.IsNullOrWhiteSpace(guarantorDto.PAN) &&
                            guarantorDto.PAN.Trim().ToUpper() == dealer.DealershipPAN?.Trim().ToUpper())
                        {
                            return new ApiResponse<string>
                            {
                                Success = false,
                                Message = $"Guarantor PAN '{guarantorDto.PAN}' cannot be same as Dealer PAN '{dealer.DealershipPAN}'"
                            };
                        }
                        var guarantor = _mapper.Map<GuarantorDetails>(guarantorDto);

                        if (guarantorDto.Id > 0)
                        {
                            var existingGuarantor = await _guarantorRepo.GetByIdAsync(guarantorDto.Id);
                            if (existingGuarantor != null)
                            {
                                _mapper.Map(guarantorDto, existingGuarantor);
                                await _guarantorRepo.UpdateAsync(existingGuarantor);
                            }
                            else
                            {
                                await _guarantorRepo.AddAsync(guarantor);
                            }
                        }
                        else
                        {
                            await _guarantorRepo.AddAsync(guarantor);
                        }
                    }
                }

                return new ApiResponse<string> { Success = true, Message = "Borrower and Guarantor details saved successfully." };
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return new ApiResponse<string> { Success = false, Message = $"Error: {message}" };
            }
        }



        #endregion
    }
}