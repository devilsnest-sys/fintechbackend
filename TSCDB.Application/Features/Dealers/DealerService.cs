using AutoMapper;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Dealer;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TscLoanManagement.TSCDB.Core.Enums;
using System.ComponentModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using TscLoanManagement.TSCDB.Infrastructure.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Dealers
{
    public class DealerService : IDealerService
    {
        private readonly IDealerRepository _dealerRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public DealerService(IDealerRepository dealerRepository, IMapper mapper, IEmailService emailService)
        {
            _dealerRepository = dealerRepository;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<IEnumerable<DealerDto>> GetAllDealersAsync()
        {
            var dealers = await _dealerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DealerDto>>(dealers);
        }

        public async Task<DealerDto> GetDealerByIdAsync(int id)
        {
            var dealer = await _dealerRepository.GetByIdAsync(id);
            if (dealer == null)
                return null;

            return _mapper.Map<DealerDto>(dealer);
        }

        public async Task<DealerDto> GetDealerByUserIdAsync(string userId)
        {
            var dealer = await _dealerRepository.GetDealerByUserIdAsync(userId);
            if (dealer == null)
                return null;

            return _mapper.Map<DealerDto>(dealer);
        }

        public async Task<DealerDto> CreateDealerAsync(DealerDto dealerDto)
        {
            try
            {
                // ✅ Uniqueness Checks
                var existingDealerByEmail = await _dealerRepository.GetByEmailAsync(dealerDto.EmailId);
                if (existingDealerByEmail != null)
                    throw new ApplicationException("Email already exists for another dealer");

                var existingDealerByPAN = await _dealerRepository.GetQueryable()
       .FirstOrDefaultAsync(d => d.DealershipPAN != null &&
                                 d.DealershipPAN.Trim().ToUpper() == dealerDto.DealershipPAN.Trim().ToUpper());

                if (existingDealerByPAN != null)
                    throw new ApplicationException("Dealership PAN already exists for another dealer");


                dealerDto.DealerCode = await GenerateDealerCodeAsync();
                dealerDto.LoanProposalNo = await GenerateLoanProposalNoAsync();

                var dealer = _mapper.Map<Dealer>(dealerDto);
                dealer.RegisteredDate = DateTime.UtcNow;
                dealer.IsActive = true;

                await _dealerRepository.AddAsync(dealer);
                return _mapper.Map<DealerDto>(dealer);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CreateDealerAsync: " + ex.Message, ex);
            }
        }



        public async Task UpdateDealerAsync(DealerDto dealerDto)
        {
            var dealer = await _dealerRepository.GetByIdAsync(dealerDto.Id);
            if (dealer == null)
                throw new ApplicationException($"Dealer with ID {dealerDto.Id} not found");

            // Map DTO to entity but preserve certain fields
            var registeredDate = dealer.RegisteredDate;
            var isActive = dealer.IsActive;

            _mapper.Map(dealerDto, dealer);

            // Restore preserved values
            dealer.RegisteredDate = registeredDate;

            // Only allow status change through the dedicated endpoint
            if (string.IsNullOrEmpty(dealerDto.Status))
                dealer.Status = DealerStatus.Pending;
            else
                dealer.Status = Enum.TryParse<DealerStatus>(dealerDto.Status, out var status) ? status : DealerStatus.Pending;


            await _dealerRepository.UpdateAsync(dealer);

            await _emailService.SendEmailAsync(
                dealer.EmailId,
                "Your Dealer Profile has been Updated",
                $"Dear {dealer.DealershipName},<br/>Your dealer profile information has been successfully updated.");

        }

        public async Task DeleteDealerAsync(int id)
        {
            var dealer = await _dealerRepository.GetByIdAsync(id);
            if (dealer == null)
                throw new ApplicationException($"Dealer with ID {id} not found");

            dealer.IsActive = false;
            await _dealerRepository.UpdateAsync(dealer);
        }

        public async Task<bool> UpdateDealerStatusAsync(UpdateDealerStatusDto dto)
        {
            var dealer = await _dealerRepository.GetByIdAsync(dto.DealerId);
            if (dealer == null)
                throw new KeyNotFoundException($"Dealer with ID {dto.DealerId} not found");

            dealer.Status = dto.NewStatus;

            if (dto.NewStatus == DealerStatus.Rejected)
            {
                if (string.IsNullOrEmpty(dto.RejectionReason))
                    throw new ArgumentException("Rejection reason is required when status is Rejected");

                dealer.RejectionReason = dto.RejectionReason;
            }
            else
            {
                dealer.RejectionReason = null; // Clear if not rejected
            }



            await _dealerRepository.UpdateAsync(dealer);

            await _emailService.SendEmailAsync(
              dealer.EmailId,
              "Your Dealer Status has been Updated",
              $"Dear {dealer.DealershipName},<br/>Your status is now: <strong>{dealer.Status}</strong>.");
            return true;
        }

        public async Task<BulkUploadResultDto> BulkUploadDealersAsync(IFormFile excelFile)
        {
            var result = new BulkUploadResultDto();
            var dealers = new List<Dealer>();

            // Set the EPPlus license mode
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet
                    int rowCount = worksheet.Dimension.Rows;

                    // Skip header row
                    result.TotalRecords = rowCount - 1;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            // Create dealer entity
                            var dealer = new Dealer
                            {
                                DealerCode = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "DealerCode")),
                                LoanProposalNo = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "LoanProposalNo")),
                                DealershipName = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "DealershipName")),
                                DealershipPAN = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "DealershipPAN")) ?? "N/A",
                                DateOfFacilityAgreement = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "DateOfFacilityAgreement")),
                                Entity = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "TypeOfEntity")) ?? "N/A",
                                CIBILOfEntity = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "CIBILOfEntity")) ?? "N/A",
                                IsActive = ParseBoolean(GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "IsActive"))),
                                GSTRegStatus = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "GSTRegStatus")) ?? "N/A",
                                GSTNo = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "GSTNo")) ?? "N/A",
                                BusinessCategory = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "BusinessCategory")) ?? "N/A",
                                MSMEStatus = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "MSMEStatus")) ?? "N/A",
                                MSMERegistrationNo = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "MSMERegistrationNo")) ?? "N/A",
                                MSMEType = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "MSMEType")) ?? "N/A",
                                BusinessType = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "BusinessType")) ?? "N/A",
                                OfficeStatus = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "OfficeStatus")) ?? "N/A",
                                AgreementDate = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "AgreementDate")),
                                AgreementExpiryDate = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "AgreementExpiryDate")),
                                ContactNo = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "ContactNo")) ?? "N/A",
                                AlternativeContactNo = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "AlternateContactNo")) ?? "N/A",
                                EmailId = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "EmailId")) ?? "N/A",
                                AlternativeEmailId = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "AlternateEmailId")) ?? "N/A",
                                ShopAddress = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "OfficeAddress")) ?? "N/A",
                                City = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "City")) ?? "N/A",
                                District = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "District")) ?? "N/A",
                                State = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "State")) ?? "N/A",
                                Pincode = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Pincode")) ?? "N/A",
                                ParkingYardAddress = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "ParkingYardAddress")) ?? "N/A",
                                ParkingStatus = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "ParkingStatus")) ?? "N/A",
                                ParkingAgreementDate = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "ParkingAgreementDate")),
                                ParkingAgreementExpiryDate = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "ParkingAgreementExpiryDate")),
                                TotalSanctionLimit = GetCellValueAsDecimal(worksheet, row, GetColumnIndexByName(worksheet, "TotalSanctionLimit")),
                                ROI = GetCellValueAsDecimal(worksheet, row, GetColumnIndexByName(worksheet, "ROI")),
                                ROIPerLakh = GetCellValueAsDecimal(worksheet, row, GetColumnIndexByName(worksheet, "ROIPerLakh")),
                                DelayROI = GetCellValueAsDecimal(worksheet, row, GetColumnIndexByName(worksheet, "DelayROI")),
                                ProcessingFee = GetCellValueAsDecimal(worksheet, row, GetColumnIndexByName(worksheet, "ProcessingFee")),
                                ProcessingCharge = GetCellValueAsDecimal(worksheet, row, GetColumnIndexByName(worksheet, "ProcessingCharge")),
                                GSTOnProcessingCharge = GetCellValueAsDecimal(worksheet, row, GetColumnIndexByName(worksheet, "GSTOnProcessingCharge")),
                                DocumentationCharge = GetCellValueAsDecimal(worksheet, row, GetColumnIndexByName(worksheet, "DocumentationCharge")),
                                GSTOnDocCharges = GetCellValueAsDecimal(worksheet, row, GetColumnIndexByName(worksheet, "GSTOnDocCharges")),
                                Status = ParseBoolean(GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "IsActive")))
                                            ? DealerStatus.Active
                                            : DealerStatus.Inactive,

                                ParkingYardPinCode = ColumnExists(worksheet, "ParkingYardPinCode")
                                        ? GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "ParkingYardPinCode"))
                                        : null,

                                ParkingYardState = ColumnExists(worksheet, "ParkingYardState")
                                        ? GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "ParkingYardState"))
                                        : null,

                                ShopPinCode = ColumnExists(worksheet, "ShopPinCode")
                                        ? GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "ShopPinCode"))
                                        : null,

                                ShopState = ColumnExists(worksheet, "ShopState")
                                        ? GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "ShopState"))
                                        : null,
                                RegisteredDate = DateTime.UtcNow,
                                RejectionReason = "N/A",
                                RelationshipManagerId = GetCellValueAsInteger(worksheet, row, GetColumnIndexByName(worksheet, "RelationshipManagerId")),
                                //UserId = 3,

                                // Initialize collections
                                BorrowerDetails = new List<BorrowerDetails>(),
                                GuarantorDetails = new List<GuarantorDetails>(),
                                ChequeDetails = new List<ChequeDetails>(),
                                SecurityDepositDetails = new List<SecurityDepositDetails>(),
                                DocumentUploads = new List<DocumentUpload>()
                            };

                            // Process Person1 (Borrower)
                            var person1Type = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person1Type"));
                            if (!string.IsNullOrEmpty(person1Type))
                            {
                                var borrower = new BorrowerDetails
                                {
                                    PersonType = person1Type, // Add this field
                                    Name = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person1Name")),
                                    PAN = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person1PAN")),
                                    AadharNo = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person1AadharNo")),
                                    DateOfBirth = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "Person1DOB")),
                                    MobileNumber = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person1Mob")),
                                    CIBILScore = GetCellValueAsInteger(worksheet, row, GetColumnIndexByName(worksheet, "Person1CIBILScore")),
                                    Email = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person1Email")),
                                    CurrentAddress = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person1CurrentAddress")),
                                    AddressStatus = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person1AddressStatus")),
                                    AddressAgreementDate = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "Person1AddressAgreementDate")),
                                    AddressAgreementExpiryDate = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "Person1AddressAgreementExpiryDate")),
                                    PermanentAddress = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person1PermanentAddress")),
                                    FatherName = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person1FatherName")),
                                    RelationshipWithEntity = "Self", // Default value for Person1
                                    IsActive = true,
                                    CreatedAt = DateTime.UtcNow
                                };
                                dealer.BorrowerDetails.Add(borrower);
                            }

                            // Process Person2
                            var person2Type = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2Type"));
                            if (!string.IsNullOrEmpty(person2Type))
                            {
                                // Check if Person2 is a Guarantor
                                if (person2Type.Equals("Guarantor", StringComparison.OrdinalIgnoreCase))
                                {
                                    var guarantor = new GuarantorDetails
                                    {
                                        PersonType = person2Type,
                                        RelationshipWithBorrower = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "RelationWithPerson1")),
                                        Name = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2Name")),
                                        PAN = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2PAN")),
                                        AadharNo = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2AadharNo")),
                                        DateOfBirth = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "Person2DOB")),
                                        MobileNumber = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2Mob")),
                                        CIBILScore = GetCellValueAsInteger(worksheet, row, GetColumnIndexByName(worksheet, "Person2CIBILScore")),
                                        Email = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2Email")),
                                        CurrentAddress = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2CurrentAddress")),
                                        AddressStatus = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2AddressStatus")),
                                        AddressAgreementDate = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "Person2AddressAgreementDate")),
                                        AddressAgreementExpiryDate = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "Person2AddressAgreementExpiryDate")),
                                        PermanentAddress = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2PermanentAddress")),
                                        FatherName = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2FatherName")),
                                        IsActive = true,
                                        CreatedAt = DateTime.UtcNow
                                    };
                                    dealer.GuarantorDetails.Add(guarantor);
                                }
                                else
                                {
                                    // Handle Person2 as another type (like Director, etc.)
                                    var additionalBorrower = new BorrowerDetails
                                    {
                                        PersonType = person2Type,
                                        Name = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2Name")),
                                        PAN = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2PAN")),
                                        AadharNo = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2AadharNo")),
                                        DateOfBirth = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "Person2DOB")),
                                        MobileNumber = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2Mob")),
                                        CIBILScore = GetCellValueAsInteger(worksheet, row, GetColumnIndexByName(worksheet, "Person2CIBILScore")),
                                        Email = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2Email")),
                                        CurrentAddress = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2CurrentAddress")),
                                        AddressStatus = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2AddressStatus")),
                                        AddressAgreementDate = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "Person2AddressAgreementDate")),
                                        AddressAgreementExpiryDate = GetCellValueAsDateTime(worksheet, row, GetColumnIndexByName(worksheet, "Person2AddressAgreementExpiryDate")),
                                        PermanentAddress = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2PermanentAddress")),
                                        FatherName = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "Person2FatherName")),
                                        RelationshipWithEntity = GetCellValueAsString(worksheet, row, GetColumnIndexByName(worksheet, "RelationWithPerson1")),
                                        IsActive = true,
                                        CreatedAt = DateTime.UtcNow
                                    };
                                    dealer.BorrowerDetails.Add(additionalBorrower);
                                }
                            }

                            // Create SecurityDepositDetails
                            var securityDeposit = new SecurityDepositDetails
                            {
                                Status = "Active", // Default status
                                Amount = 0, // Initialize with 0, can be updated later
                                CIBILOfEntity = dealer.CIBILOfEntity,
                                TotalSanctionLimit = dealer.TotalSanctionLimit,
                                ROI = dealer.ROI,
                                ROIPerLakh = dealer.ROIPerLakh,
                                DelayROI = dealer.DelayROI,
                                ProcessingFee = dealer.ProcessingFee,
                                ProcessingCharge = dealer.ProcessingCharge,
                                GSTOnProcessingCharge = dealer.GSTOnProcessingCharge,
                                DocumentationCharge = dealer.DocumentationCharge,
                                GSTOnDocCharges = dealer.GSTOnDocCharges,
                                IsActive = true,
                                RegisteredDate = DateTime.UtcNow,
                                Remarks = "Created from bulk upload"
                            };
                            dealer.SecurityDepositDetails.Add(securityDeposit);

                            // Basic validation
                            if (string.IsNullOrEmpty(dealer.DealershipName))
                            {
                                result.Errors.Add($"Row {row}: Dealership Name is required");
                                result.FailureCount++;
                                continue;
                            }

                            dealers.Add(dealer);
                            result.SuccessCount++;
                        }
                        catch (Exception ex)
                        {
                            result.Errors.Add($"Row {row}: {ex.Message}");
                            result.FailureCount++;
                        }
                    }
                }
            }

            // Save valid dealers to database
            if (dealers.Any())
            {
                await _dealerRepository.BulkInsertAsync(dealers);
            }

            return result;
        }

        private int GetColumnIndexByName(ExcelWorksheet worksheet, string columnName)
        {
            // First row contains headers
            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                if (worksheet.Cells[1, col].Value?.ToString() == columnName)
                {
                    return col;
                }
            }
            // Return -1 if column not found (will cause exception which is caught in the calling method)
            return -1;
        }


        private string GetCellValueAsString(ExcelWorksheet worksheet, int row, int column)
        {
            if (column < 1) return null; // Column not found

            var cellValue = worksheet.Cells[row, column].Value;
            return cellValue?.ToString()?.Trim();
        }

        private DateTime? GetCellValueAsDateTime(ExcelWorksheet worksheet, int row, int column)
        {
            if (column < 1) return null; // Column not found

            var cellValue = worksheet.Cells[row, column].Value;
            if (cellValue == null)
                return null;

            // First try to parse as Excel date
            if (cellValue is double numericDate)
            {
                try
                {
                    return DateTime.FromOADate(numericDate);
                }
                catch
                {
                    // Fall through to string parsing
                }
            }

            // Try parsing as string in various formats
            var dateStr = cellValue.ToString().Trim();

            // Try standard date format
            if (DateTime.TryParse(dateStr, out DateTime date))
                return date;

            // Try dd-MM-yyyy format
            if (DateTime.TryParseExact(dateStr, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateExact))
                return dateExact;

            return null;
        }

        private decimal? GetCellValueAsDecimal(ExcelWorksheet worksheet, int row, int column)
        {
            if (column < 1) return null; // Column not found

            var cellValue = worksheet.Cells[row, column].Value;
            if (cellValue == null)
                return null;

            if (decimal.TryParse(cellValue.ToString(), out decimal value))
                return value;

            return null;
        }

        private int? GetCellValueAsInteger(ExcelWorksheet worksheet, int row, int column)
        {
            if (column <= 0)
                return null;

            var cellValue = worksheet.Cells[row, column].Value;
            if (cellValue == null)
                return null;

            if (int.TryParse(cellValue.ToString(), out int result))
                return result;

            return null;
        }

        private bool ParseBoolean(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.ToLower().Trim();

            return value == "true" || value == "yes" || value == "1" || value == "y";
        }

        private bool ColumnExists(ExcelWorksheet worksheet, string columnName)
        {
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var header = worksheet.Cells[1, col].Text?.Trim();
                if (!string.IsNullOrEmpty(header) && header.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<string> GenerateDealerCodeAsync()
        {
            var latestDealer = await _dealerRepository
                .GetQueryable()
                .OrderByDescending(d => d.DealerCode)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (latestDealer != null && !string.IsNullOrEmpty(latestDealer.DealerCode))
            {
                var numericPart = new string(latestDealer.DealerCode.SkipWhile(c => !char.IsDigit(c)).ToArray());

                if (int.TryParse(numericPart, out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            return $"TSC{nextNumber:D3}";
        }


        public async Task<string> GenerateLoanProposalNoAsync()
        {
            var latestDealer = await _dealerRepository
                .GetQueryable()
                .OrderByDescending(d => d.LoanProposalNo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (latestDealer != null && !string.IsNullOrEmpty(latestDealer.LoanProposalNo))
            {
                var numericPart = new string(latestDealer.LoanProposalNo.SkipWhile(c => !char.IsDigit(c)).ToArray());

                if (int.TryParse(numericPart, out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            return $"TSCF-{nextNumber:D3}";
        }






    }
}