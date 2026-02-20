using System.Formats.Asn1;
using AutoMapper;
using System.Globalization;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Loan;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;
using CsvHelper;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using DocumentFormat.OpenXml.InkML;
using TscLoanManagement.TSCDB.Core.Domain.Dealer;
using TscLoanManagement.TSCDB.Infrastructure.Data.Context;


namespace TscLoanManagement.TSCDB.Application.Features.Loans
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IDealerRepository _dealerRepository;
        private readonly ILoanCalculationService _loanCalculationService;
        private readonly IMapper _mapper;
        private readonly TSCDbContext _context;

        public LoanService(ILoanRepository loanRepository, IDealerRepository dealerRepository, IMapper mapper, ILoanCalculationService loanCalculationService, TSCDbContext context)
        {
            _loanRepository = loanRepository;
            _dealerRepository = dealerRepository;
            _mapper = mapper;
            _loanCalculationService = loanCalculationService;
            _context = context;
        }

        public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<IEnumerable<LoanDto>> GetLoansByDealerIdAsync(int dealerId)
        {
            var loans = await _loanRepository.GetLoansByDealerIdAsync(dealerId);
            var loanDtos = _mapper.Map<IEnumerable<LoanDto>>(loans).ToList();

            for (int i = 0; i < loanDtos.Count; i++)
            {
                if (loanDtos[i].LoanDetailId == null && loans.ElementAt(i).LoanDetailId.HasValue)
                {
                    loanDtos[i].LoanDetailId = loans.ElementAt(i).LoanDetailId.Value;
                }
            }

            return loanDtos;
        }


        public async Task<LoanDto> GetLoanByIdAsync(int id)
        {
            //var loan = await _loanRepository.GetLoanWithDetailsAsync(id);
            //return _mapper.Map<LoanDto>(loan);

            var loan = await _loanRepository.GetLoanWithDetailsAsync(id);
            var loanDto = _mapper.Map<LoanDto>(loan);

            // If LoanDetailId is still null, try to fetch it directly
            if (loanDto.LoanDetailId == null && loan.LoanDetailId.HasValue)
            {
                loanDto.LoanDetailId = loan.LoanDetailId.Value;
            }

            return loanDto;
        }

        public async Task<LoanDto> CreateLoanAsync(CreateUpdateLoanDto loanDto)
        {
            var loan = _mapper.Map<Loan>(loanDto);
            loan.CreatedDate = DateTime.UtcNow;
            loan.IsActive = true;
            loan.LoanNumber = await GenerateLoanNumberAsync();

            await _loanRepository.AddAsync(loan);

            try
            {
                var createLoanRequest = new CreateLoanRequestDto
                {
                    CustomerId = loan.DealerId,
                    PrincipalAmount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    ProcessingFeeRate = loan.ProcessingFee,
                    GSTPercent = (decimal)loan.GSTOnProcessingFee,
                    StartDate = loan.DateOfWithdraw,
                    DueDays = 60,
                    DelayInterestRate = (decimal)loan.DelayedROI
                };

                var loanDetail = await _loanCalculationService.CreateLoanAsync(createLoanRequest);

                loan.LoanDetailId = loanDetail.LoanId;
                await _loanRepository.UpdateAsync(loan);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create loan detail for loan {loan.Id}: {ex.Message}");
            }

            // 👉 Map after LoanDetailId is updated
            var createdLoanDto = _mapper.Map<LoanDto>(loan);

            return createdLoanDto;
        }


        public async Task UpdateLoanAsync(CreateUpdateLoanDto loanDto)
        {
            var loan = await _loanRepository.GetLoanWithDetailsAsync(loanDto.Id);
            if (loan == null)
                throw new ApplicationException($"Loan with ID {loanDto.Id} not found");

            _mapper.Map(loanDto, loan);
            await _loanRepository.UpdateAsync(loan);

            // Map the created loan to DTO to get the generated ID
            var createdLoanDto = _mapper.Map<LoanDto>(loan);

            // Create corresponding entry in LoanCalculationService
            try
            {
                var createLoanRequest = new CreateLoanRequestDto
                {
                    CustomerId = loan.DealerId, // Using DealerId as CustomerId - adjust if you have a separate CustomerId
                    PrincipalAmount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    ProcessingFeeRate = loan.ProcessingFee, // Calculate percentage from absolute fee
                    GSTPercent = (decimal)loan.GSTOnProcessingFee, // Calculate GST percentage
                    StartDate = loan.DateOfWithdraw,
                    DueDays = 60, // Calculate days between withdrawal and due date
                    DelayInterestRate = (decimal)loan.DelayedROI
                };

                var loanDetail = await _loanCalculationService.CreateLoanAsync(createLoanRequest);

                // Optionally, you can store the LoanDetail ID in the main loan entity
                // This would require adding a property to the Loan entity
                // loan.LoanDetailId = loanDetail.LoanId;
                // await _loanRepository.UpdateAsync(loan);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the main loan creation
                // You might want to use a proper logging framework here
                Console.WriteLine($"Failed to create loan detail for loan {loan.Id}: {ex.Message}");
                // Optionally, you could rollback the main loan creation or handle this differently
            }
        }

        public async Task DeleteLoanAsync(int id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
                throw new ApplicationException($"Loan with ID {id} not found");

            loan.IsActive = false;
            await _loanRepository.UpdateAsync(loan);
        }

        public async Task<bool> BulkUploadLoansAsync(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file == null || file.Length == 0)
                throw new ApplicationException("File is empty");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
                throw new ApplicationException("Only Excel (.xlsx) files are supported");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new ApplicationException("No worksheet found");

            int rowCount = worksheet.Dimension.Rows;
            int colCount = worksheet.Dimension.Columns;

            // 1. Build header map
            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int col = 1; col <= colCount; col++)
            {
                var header = worksheet.Cells[1, col].Text?.Trim();
                if (!string.IsNullOrEmpty(header) && !headerMap.ContainsKey(header))
                {
                    headerMap[header] = col;
                }
            }

            // 2. Required columns check (excluding payments since they are dynamic)
            var requiredCols = new[]
            {
        "LoanId","LoanNumber","DealerCode","DealershipName","Status","DateOfWithdraw",
        "Amount","UTRNumber","InterestRate","DelayedROI","ProcessingFee","GSTOnProcessingFee",
        "Processingfeereceiveddate","DueDate","SettledDate",
        "Make","Model","RegistrationNumber","Year","ChassisNumber","EngineNumber","Value",
        "Purchasesource","InvoiceNumber","InvoiceDate","WaiverType","InterestWaiver"
    };

            var missingCols = requiredCols.Where(c => !headerMap.ContainsKey(c)).ToList();
            if (missingCols.Any())
                throw new ApplicationException($"Missing required columns: {string.Join(", ", missingCols)}");

            // Helper to read cells
            string GetCell(int row, string columnName)
            {
                return headerMap.TryGetValue(columnName, out var colIndex)
                    ? worksheet.Cells[row, colIndex].Text
                    : string.Empty;
            }

            // Process each row
            for (int row = 2; row <= rowCount; row++)
            {
                var record = new LoanImportDto
                {
                    LoanId = GetCell(row, "LoanId"),
                    LoanNumber = GetCell(row, "LoanNumber"),
                    DealerCode = GetCell(row, "DealerCode"),
                    DealershipName = GetCell(row, "DealershipName"),
                    Status = GetCell(row, "Status"),
                    DateOfWithdraw = TryDate(GetCell(row, "DateOfWithdraw")).GetValueOrDefault(),
                    Amount = TryDecimal(GetCell(row, "Amount")) ?? 0m,
                    UtrNumber = GetCell(row, "UTRNumber"),
                    InterestRate = TryDecimal(GetCell(row, "InterestRate")) ?? 0m,
                    DelayedROI = TryDecimal(GetCell(row, "DelayedROI")) ?? 0m,
                    ProcessingFee = TryDecimal(GetCell(row, "ProcessingFee")) ?? 0m,
                    GSTOnProcessingFee = TryDecimal(GetCell(row, "GSTOnProcessingFee")),
                    ProcessingFeeReceivedDate = TryDate(GetCell(row, "Processingfeereceiveddate")),
                    DueDate = TryDate(GetCell(row, "DueDate")).GetValueOrDefault(),
                    SettledDate = TryDate(GetCell(row, "SettledDate")),
                    VehicleMake = GetCell(row, "Make"),
                    VehicleModel = GetCell(row, "Model"),
                    VehicleRegistrationNumber = GetCell(row, "RegistrationNumber"),
                    VehicleYear = TryInt(GetCell(row, "Year")) ?? 0,
                    VehicleChassisNumber = GetCell(row, "ChassisNumber"),
                    VehicleEngineNumber = GetCell(row, "EngineNumber"),
                    VehicleValue = TryDecimal(GetCell(row, "Value")) ?? 0m,
                    PurchaseSource = GetCell(row, "Purchasesource"),
                    InvoiceNumber = GetCell(row, "InvoiceNumber"),
                    InvoiceDate = TryDate(GetCell(row, "InvoiceDate")),
                    InterestWaiver = TryDecimal(GetCell(row, "InterestWaiver")),
                    WaiverType = GetCell(row, "WaiverType"),
                };

                // ✅ Dynamically read up to 15 payments
                for (int i = 1; i <= 15; i++)
                {
                    var payment = new PaymentRecordDto
                    {
                        Date = TryDate(GetCell(row, $"Payment{i}Date")),
                        Received = TryDecimal(GetCell(row, $"PaymentRecieved{i}")),
                        InterestEarned = TryDecimal(GetCell(row, $"InterestEarned{i}")),
                        DelayedInterestEarned = TryDecimal(GetCell(row, $"DelayedInterestEarned{i}")),
                        PrincipalReceived = TryDecimal(GetCell(row, $"PrincipalRecieved{i}"))
                    };

                    if (payment.Date.HasValue || payment.Received.HasValue ||
                        payment.InterestEarned.HasValue || payment.DelayedInterestEarned.HasValue ||
                        payment.PrincipalReceived.HasValue)
                    {
                        record.Payments.Add(payment);
                    }
                }

                // === Create Loan ===
                var dealer = await _dealerRepository.GetByDealerCodeAsync(record.DealerCode);
                if (dealer == null)
                    throw new ApplicationException($"Dealer with code '{record.DealerCode}' not found at row {row}");

                var loan = new Loan
                {
                    LoanNumber = record.LoanNumber,
                    DateOfWithdraw = record.DateOfWithdraw,
                    Amount = record.Amount,
                    InterestRate = record.InterestRate,
                    DelayedROI = record.DelayedROI,
                    DealerId = dealer.Id,
                    UtrNumber = record.UtrNumber,
                    ProcessingFee = record.ProcessingFee,
                    GSTOnProcessingFee = record.GSTOnProcessingFee,
                    ProcessingFeeReceivedDate = record.ProcessingFeeReceivedDate,
                    DueDate = record.DueDate,
                    SettledDate = record.SettledDate,
                    PurchaseSource = record.PurchaseSource,
                    InvoiceNumber = record.InvoiceNumber,
                    InvoiceDate = record.InvoiceDate,
                    InterestWaiver = record.InterestWaiver,
                    Status = record.Status,
                    IsActive = record.Status?.ToLower() != "closed",
                    CreatedDate = DateTime.UtcNow,
                    VehicleInfo = new VehicleInfo
                    {
                        Make = record.VehicleMake,
                        Model = record.VehicleModel,
                        RegistrationNumber = record.VehicleRegistrationNumber,
                        Year = record.VehicleYear,
                        ChassisNumber = record.VehicleChassisNumber,
                        EngineNumber = record.VehicleEngineNumber,
                        Value = record.VehicleValue
                    },
                    LoanPayments = new List<LoanPayment>()
                };

                // Add loan payments
                foreach (var p in record.Payments)
                {
                    AddPaymentIfExists(p.Date, p.Received, p.InterestEarned, p.DelayedInterestEarned, p.PrincipalReceived, loan);
                }

                await _loanRepository.AddAsync(loan);

                // === Loan Calculation Service Integration (same as before) ===
                try
                {
                    var createLoanRequest = new CreateLoanRequestDto
                    {
                        CustomerId = dealer.Id,
                        PrincipalAmount = record.Amount,
                        InterestRate = record.InterestRate,
                        ProcessingFeeRate = CalculateProcessingFeeRate(record.ProcessingFee, record.Amount),
                        GSTPercent = CalculateGSTPercent(record.GSTOnProcessingFee, record.ProcessingFee),
                        StartDate = record.DateOfWithdraw,
                        DueDays = 60,
                        DelayInterestRate = record.DelayedROI,
                        Status = record.Status
                    };

                    var loanDetail = await _loanCalculationService.CreateLoanAsync(createLoanRequest);
                    loan.LoanDetailId = loanDetail.LoanId;
                    await _loanRepository.UpdateAsync(loan);

                    if (record.ProcessingFee > 0)
                    {
                        await _loanCalculationService.AddBulkLoanFeeAsync(new BulkLoanFeeDto
                        {
                            LoanId = loanDetail.LoanId,
                            FeeAmount = record.ProcessingFee,
                            GSTAmount = record.GSTOnProcessingFee ?? 0,
                            TotalFeeWithGST = record.ProcessingFee + (record.GSTOnProcessingFee ?? 0)
                        });
                    }
                    bool processingFeeAdded = false;
                    foreach (var p in record.Payments)
                    {
                        if (p.Date.HasValue && p.Received.HasValue)
                        {
                            await _loanCalculationService.AddBulkInstallmentAsync(new BulkInstallmentDto
                            {
                                LoanId = loanDetail.LoanId,
                                PaidDate = p.Date.Value,
                                AmountPaid = p.Received.Value,
                                AdjustedToPrincipal = p.PrincipalReceived ?? 0,
                                AdjustedToInterest = p.InterestEarned ?? 0,
                                AdjustedToDelayInterest = p.DelayedInterestEarned ?? 0,
                                AdjustedToFee = !processingFeeAdded ? (decimal)(record.ProcessingFee + record.GSTOnProcessingFee) : 0,
                                DueFee = 0,
                                DueInterest = 0
                            });
                            processingFeeAdded = true;
                        }
                    }

                    if (record.InterestWaiver.HasValue && record.InterestWaiver.Value > 0 && !string.IsNullOrWhiteSpace(record.WaiverType))
                    {
                        await _loanCalculationService.WaiveLoanComponentAsync(loanDetail.LoanId, new WaiverRequestDto
                        {
                            WaiverType = record.WaiverType,
                            Amount = record.InterestWaiver.Value,
                            Reason = $"Bulk import waiver of type {record.WaiverType}"
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create loan detail for bulk upload row {row}: {ex.Message}");
                }
            }

            return true;
        }




        //public async Task<bool> BulkUploadLoansAsync(IFormFile file)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    if (file == null || file.Length == 0)
        //        throw new ApplicationException("File is empty");

        //    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        //    if (extension != ".xlsx")
        //        throw new ApplicationException("Only Excel (.xlsx) files are supported");

        //    using var stream = new MemoryStream();
        //    await file.CopyToAsync(stream);
        //    using var package = new ExcelPackage(stream);
        //    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //    if (worksheet == null)
        //        throw new ApplicationException("No worksheet found");

        //    int rowCount = worksheet.Dimension.Rows;
        //    int colCount = worksheet.Dimension.Columns;

        //    // 1. Build header map
        //    var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        //    for (int col = 1; col <= colCount; col++)
        //    {
        //        var header = worksheet.Cells[1, col].Text?.Trim();
        //        if (!string.IsNullOrEmpty(header) && !headerMap.ContainsKey(header))
        //        {
        //            headerMap[header] = col;
        //        }
        //    }

        //    // 2. Ensure required columns exist
        //    var requiredCols = new[]
        //    {
        //    "LoanId","LoanNumber","DealerCode","DealershipName","Status","DateOfWithdraw",
        //    "Amount","UTRNumber","InterestRate","DelayedROI","ProcessingFee","GSTOnProcessingFee",
        //    "Processing fee received date","DueDate","SettledDate",
        //    "Make","Model","RegistrationNumber","Year","ChassisNumber","EngineNumber","Value",
        //    "Purchase source","InvoiceNumber","InvoiceDate",
        //    "WaiverType","InterestWaiver",
        //    "Payment1Date","PaymentRecieved1","InterestEarned1","DelayedInterestEarned1","PrincipalRecieved1",
        //    "Payment2Date","PaymentRecieved2","InterestEarned2","DelayedInterestEarned2","PrincipalRecieved2",
        //    "Payment3Date","PaymentRecieved3","InterestEarned3","DelayedInterestEarned3","PrincipalRecieved3"
        //};


        //    var missingCols = requiredCols.Where(c => !headerMap.ContainsKey(c)).ToList();
        //    if (missingCols.Any())
        //        throw new ApplicationException($"Missing required columns: {string.Join(", ", missingCols)}");

        //    // 3. Helper to read cells safely
        //    string GetCell(int row, string columnName)
        //    {
        //        return headerMap.TryGetValue(columnName, out var colIndex)
        //            ? worksheet.Cells[row, colIndex].Text
        //            : string.Empty;
        //    }

        //    // 4. Process each row
        //    for (int row = 2; row <= rowCount; row++)
        //    {
        //        var record = new LoanImportDto
        //        {
        //            LoanId = GetCell(row, "LoanId"),
        //            LoanNumber = GetCell(row, "LoanNumber"),
        //            DealerCode = GetCell(row, "DealerCode"),
        //            DealershipName = GetCell(row, "DealershipName"),
        //            Status = GetCell(row, "Status"),
        //            DateOfWithdraw = TryDate(GetCell(row, "DateOfWithdraw")).GetValueOrDefault(),
        //            Amount = TryDecimal(GetCell(row, "Amount")) ?? 0m,
        //            UtrNumber = GetCell(row, "UTRNumber"),
        //            InterestRate = TryDecimal(GetCell(row, "InterestRate")) ?? 0m,
        //            DelayedROI = TryDecimal(GetCell(row, "DelayedROI")) ?? 0m,
        //            ProcessingFee = TryDecimal(GetCell(row, "ProcessingFee")) ?? 0m,
        //            GSTOnProcessingFee = TryDecimal(GetCell(row, "GSTOnProcessingFee")),
        //            ProcessingFeeReceivedDate = TryDate(GetCell(row, "Processingfeereceiveddate")),
        //            DueDate = TryDate(GetCell(row, "DueDate")).GetValueOrDefault(),
        //            SettledDate = TryDate(GetCell(row, "SettledDate")),

        //            VehicleMake = GetCell(row, "Make"),
        //            VehicleModel = GetCell(row, "Model"),
        //            VehicleRegistrationNumber = GetCell(row, "RegistrationNumber"),
        //            VehicleYear = TryInt(GetCell(row, "Year")) ?? 0,
        //            VehicleChassisNumber = GetCell(row, "ChassisNumber"),
        //            VehicleEngineNumber = GetCell(row, "EngineNumber"),
        //            VehicleValue = TryDecimal(GetCell(row, "Value")) ?? 0m,

        //            PurchaseSource = GetCell(row, "Purchasesource"),
        //            InvoiceNumber = GetCell(row, "InvoiceNumber"),
        //            InvoiceDate = TryDate(GetCell(row, "InvoiceDate")),
        //            InterestWaiver = TryDecimal(GetCell(row, "InterestWaiver")),
        //            WaiverType = GetCell(row, "WaiverType"),

        //            Payment1Date = TryDate(GetCell(row, "Payment1Date")),
        //            PaymentRecieved1 = TryDecimal(GetCell(row, "PaymentRecieved1")),
        //            InterestEarned1 = TryDecimal(GetCell(row, "InterestEarned1")),
        //            DelayedInterestEarned1 = TryDecimal(GetCell(row, "DelayedInterestEarned1")),
        //            PrincipalRecieved1 = TryDecimal(GetCell(row, "PrincipalRecieved1")),

        //            Payment2Date = TryDate(GetCell(row, "Payment2Date")),
        //            PaymentRecieved2 = TryDecimal(GetCell(row, "PaymentRecieved2")),
        //            InterestEarned2 = TryDecimal(GetCell(row, "InterestEarned2")),
        //            DelayedInterestEarned2 = TryDecimal(GetCell(row, "DelayedInterestEarned2")),
        //            PrincipalRecieved2 = TryDecimal(GetCell(row, "PrincipalRecieved2")),

        //            Payment3Date = TryDate(GetCell(row, "Payment3Date")),
        //            PaymentRecieved3 = TryDecimal(GetCell(row, "PaymentRecieved3")),
        //            InterestEarned3 = TryDecimal(GetCell(row, "InterestEarned3")),
        //            DelayedInterestEarned3 = TryDecimal(GetCell(row, "DelayedInterestEarned3")),
        //            PrincipalRecieved3 = TryDecimal(GetCell(row, "PrincipalRecieved3"))
        //        };

        //        //Get dealer by dealer code
        //        var dealer = await _dealerRepository.GetByDealerCodeAsync(record.DealerCode);
        //        if (dealer == null)
        //        {
        //            throw new ApplicationException($"Dealer with code '{record.DealerCode}' not found at row {row}");
        //        }

        //        var loan = new Loan
        //        {
        //            LoanNumber = record.LoanNumber,
        //            DateOfWithdraw = record.DateOfWithdraw,
        //            Amount = record.Amount,
        //            InterestRate = record.InterestRate,
        //            DelayedROI = record.DelayedROI,
        //            DealerId = dealer.Id,
        //            UtrNumber = record.UtrNumber,
        //            ProcessingFee = record.ProcessingFee,
        //            GSTOnProcessingFee = record.GSTOnProcessingFee,
        //            ProcessingFeeReceivedDate = record.ProcessingFeeReceivedDate,
        //            DueDate = record.DueDate,
        //            SettledDate = record.SettledDate,
        //            PurchaseSource = record.PurchaseSource,
        //            InvoiceNumber = record.InvoiceNumber,
        //            InvoiceDate = record.InvoiceDate,
        //            InterestWaiver = record.InterestWaiver,
        //            Status = record.Status,
        //            IsActive = record.Status?.ToLower() != "closed",
        //            CreatedDate = DateTime.UtcNow,

        //            VehicleInfo = new VehicleInfo
        //            {
        //                Make = record.VehicleMake,
        //                Model = record.VehicleModel,
        //                RegistrationNumber = record.VehicleRegistrationNumber,
        //                Year = record.VehicleYear,
        //                ChassisNumber = record.VehicleChassisNumber,
        //                EngineNumber = record.VehicleEngineNumber,
        //                Value = record.VehicleValue
        //            },

        //            LoanPayments = new List<LoanPayment>()
        //        };

        //        // Add Loan Payments
        //        AddPaymentIfExists(record.Payment1Date, record.PaymentRecieved1, record.InterestEarned1, record.DelayedInterestEarned1, record.PrincipalRecieved1, loan);
        //        AddPaymentIfExists(record.Payment2Date, record.PaymentRecieved2, record.InterestEarned2, record.DelayedInterestEarned2, record.PrincipalRecieved2, loan);
        //        AddPaymentIfExists(record.Payment3Date, record.PaymentRecieved3, record.InterestEarned3, record.DelayedInterestEarned3, record.PrincipalRecieved3, loan);

        //        await _loanRepository.AddAsync(loan);

        //        // Create corresponding entry in LoanCalculationService for bulk upload
        //        try
        //        {
        //            var createLoanRequest = new CreateLoanRequestDto
        //            {
        //                CustomerId = dealer.Id,
        //                PrincipalAmount = record.Amount,
        //                InterestRate = record.InterestRate,
        //                ProcessingFeeRate = CalculateProcessingFeeRate(record.ProcessingFee, record.Amount),
        //                GSTPercent = CalculateGSTPercent(record.GSTOnProcessingFee, record.ProcessingFee),
        //                StartDate = record.DateOfWithdraw,
        //                DueDays = 60,
        //                DelayInterestRate = record.DelayedROI,
        //                Status = record.Status
        //            };

        //            var loanDetail = await _loanCalculationService.CreateLoanAsync(createLoanRequest);

        //            loan.LoanDetailId = loanDetail.LoanId;
        //            await _loanRepository.UpdateAsync(loan);

        //            if (record.ProcessingFee > 0)
        //            {
        //                await _loanCalculationService.AddBulkLoanFeeAsync(new BulkLoanFeeDto
        //                {
        //                    LoanId = loanDetail.LoanId,
        //                    FeeAmount = record.ProcessingFee,
        //                    GSTAmount = record.GSTOnProcessingFee ?? 0,
        //                    TotalFeeWithGST = record.ProcessingFee + (record.GSTOnProcessingFee ?? 0)
        //                    //FeeReceivedDate = record.ProcessingFeeReceivedDate
        //                });
        //            }
        //            // Add bulk installments with breakdowns
        //            if (record.Payment1Date.HasValue && record.PaymentRecieved1.HasValue)
        //            {
        //                await _loanCalculationService.AddBulkInstallmentAsync(new BulkInstallmentDto
        //                {
        //                    LoanId = loanDetail.LoanId,
        //                    PaidDate = record.Payment1Date.Value,
        //                    AmountPaid = record.PaymentRecieved1.Value,
        //                    AdjustedToPrincipal = record.PrincipalRecieved1 ?? 0,
        //                    AdjustedToInterest = record.InterestEarned1 ?? 0,
        //                    AdjustedToDelayInterest = record.DelayedInterestEarned1 ?? 0,
        //                    AdjustedToFee = record.ProcessingFee + (record.GSTOnProcessingFee ?? 0),
        //                    DueFee = 0,
        //                    DueInterest = 0
        //                });
        //            }

        //            if (record.Payment2Date.HasValue && record.PaymentRecieved2.HasValue)
        //            {
        //                await _loanCalculationService.AddBulkInstallmentAsync(new BulkInstallmentDto
        //                {
        //                    LoanId = loanDetail.LoanId,
        //                    PaidDate = record.Payment2Date.Value,
        //                    AmountPaid = record.PaymentRecieved2.Value,
        //                    AdjustedToPrincipal = record.PrincipalRecieved2 ?? 0,
        //                    AdjustedToInterest = record.InterestEarned2 ?? 0,
        //                    AdjustedToDelayInterest = record.DelayedInterestEarned2 ?? 0,
        //                    AdjustedToFee = 0,
        //                    DueFee = 0,
        //                    DueInterest = 0
        //                });
        //            }

        //            if (record.Payment3Date.HasValue && record.PaymentRecieved3.HasValue)
        //            {
        //                await _loanCalculationService.AddBulkInstallmentAsync(new BulkInstallmentDto
        //                {
        //                    LoanId = loanDetail.LoanId,
        //                    PaidDate = record.Payment3Date.Value,
        //                    AmountPaid = record.PaymentRecieved3.Value,
        //                    AdjustedToPrincipal = record.PrincipalRecieved3 ?? 0,
        //                    AdjustedToInterest = record.InterestEarned3 ?? 0,
        //                    AdjustedToDelayInterest = record.DelayedInterestEarned3 ?? 0,
        //                    AdjustedToFee = 0,
        //                    DueFee = 0,
        //                    DueInterest = 0
        //                });
        //            }

        //            if (record.InterestWaiver.HasValue
        //                && record.InterestWaiver.Value > 0
        //                && !string.IsNullOrWhiteSpace(record.WaiverType))
        //            {
        //                var waiver = new WaiverDto
        //                {
        //                    LoanId = loanDetail.LoanId,          // << demanded mapping
        //                    InstallmentId = null,                    // bulk load has no instal‑id yet
        //                    WaiverType = record.WaiverType.Trim(),   // “ProcessingFee”, “Interest”, …
        //                    Amount = record.InterestWaiver.Value,
        //                    Reason = "Imported via bulk upload",
        //                    CreatedAt = DateTime.UtcNow
        //                };

        //                await _loanCalculationService.WaiveLoanComponentAsync(loanDetail.LoanId, new WaiverRequestDto
        //                {
        //                    WaiverType = record.WaiverType,
        //                    Amount = record.InterestWaiver.Value,
        //                    Reason = $"Bulk import waiver of type {record.WaiverType}"
        //                });

        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Failed to create loan detail for bulk upload row {row}: {ex.Message}");
        //            // Continue processing other rows
        //        }
        //    }

        //    return true;
        //}


        //public async Task<bool> BulkUploadLoansAsync(IFormFile file)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    if (file == null || file.Length == 0)
        //        throw new ApplicationException("File is empty");

        //    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        //    if (extension != ".xlsx")
        //        throw new ApplicationException("Only Excel (.xlsx) files are supported");

        //    using var stream = new MemoryStream();
        //    await file.CopyToAsync(stream);
        //    using var package = new ExcelPackage(stream);
        //    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //    if (worksheet == null)
        //        throw new ApplicationException("No worksheet found");

        //    int rowCount = worksheet.Dimension.Rows;

        //    for (int row = 2; row <= rowCount; row++)
        //    {
        //        var record = new LoanImportDto
        //        {
        //            LoanId = worksheet.Cells[row, 1].Text,
        //            LoanNumber = worksheet.Cells[row, 2].Text,
        //            DealerCode = worksheet.Cells[row, 3].Text,
        //            DealershipName = worksheet.Cells[row, 4].Text,
        //            Status = worksheet.Cells[row, 5].Text,
        //            DateOfWithdraw = TryDate(worksheet.Cells[row, 6].Text).GetValueOrDefault(),
        //            Amount = TryDecimal(worksheet.Cells[row, 7].Text) ?? 0m,
        //            UtrNumber = worksheet.Cells[row, 8].Text,
        //            InterestRate = TryDecimal(worksheet.Cells[row, 9].Text) ?? 0m,
        //            DelayedROI = TryDecimal(worksheet.Cells[row, 10].Text) ?? 0m,
        //            ProcessingFee = TryDecimal(worksheet.Cells[row, 11].Text) ?? 0m,
        //            GSTOnProcessingFee = TryDecimal(worksheet.Cells[row, 12].Text),
        //            ProcessingFeeReceivedDate = TryDate(worksheet.Cells[row, 13].Text),
        //            DueDate = TryDate(worksheet.Cells[row, 14].Text).GetValueOrDefault(),
        //            SettledDate = TryDate(worksheet.Cells[row, 15].Text),

        //            VehicleMake = worksheet.Cells[row, 16].Text,
        //            VehicleModel = worksheet.Cells[row, 17].Text,
        //            VehicleRegistrationNumber = worksheet.Cells[row, 18].Text,
        //            VehicleYear = TryInt(worksheet.Cells[row, 19].Text) ?? 0,
        //            VehicleChassisNumber = worksheet.Cells[row, 20].Text,
        //            VehicleEngineNumber = worksheet.Cells[row, 21].Text,
        //            VehicleValue = TryDecimal(worksheet.Cells[row, 22].Text) ?? 0m,

        //            PurchaseSource = worksheet.Cells[row, 23].Text,
        //            InvoiceNumber = worksheet.Cells[row, 24].Text,
        //            InvoiceDate = TryDate(worksheet.Cells[row, 25].Text),   
        //            InterestWaiver = TryDecimal(worksheet.Cells[row, 26].Text),
        //            WaiverType = worksheet.Cells[row, 27].Text,

        //            Payment1Date = TryDate(worksheet.Cells[row, 28].Text),
        //            PaymentRecieved1 = TryDecimal(worksheet.Cells[row, 29].Text),
        //            InterestEarned1 = TryDecimal(worksheet.Cells[row, 30].Text),
        //            DelayedInterestEarned1 = TryDecimal(worksheet.Cells[row, 31].Text),
        //            PrincipalRecieved1 = TryDecimal(worksheet.Cells[row, 32].Text),

        //            Payment2Date = TryDate(worksheet.Cells[row, 33].Text),
        //            PaymentRecieved2 = TryDecimal(worksheet.Cells[row, 34].Text),
        //            InterestEarned2 = TryDecimal(worksheet.Cells[row, 35].Text),
        //            DelayedInterestEarned2 = TryDecimal(worksheet.Cells[row, 36].Text),
        //            PrincipalRecieved2 = TryDecimal(worksheet.Cells[row, 37].Text),

        //            Payment3Date = TryDate(worksheet.Cells[row, 38].Text),
        //            PaymentRecieved3 = TryDecimal(worksheet.Cells[row, 39].Text),
        //            InterestEarned3 = TryDecimal(worksheet.Cells[row, 40].Text),
        //            DelayedInterestEarned3 = TryDecimal(worksheet.Cells[row, 41].Text),
        //            PrincipalRecieved3 = TryDecimal(worksheet.Cells[row, 42].Text), 
        //        };

        //        // Get dealer by dealer code
        //        var dealer = await _dealerRepository.GetByDealerCodeAsync(record.DealerCode);
        //        if (dealer == null)
        //        {
        //            throw new ApplicationException($"Dealer with code '{record.DealerCode}' not found at row {row}");
        //        }

        //        var loan = new Loan
        //        {
        //            LoanNumber = record.LoanNumber,
        //            DateOfWithdraw = record.DateOfWithdraw,
        //            Amount = record.Amount,
        //            InterestRate = record.InterestRate,
        //            DelayedROI = record.DelayedROI,
        //            DealerId = dealer.Id,
        //            UtrNumber = record.UtrNumber,
        //            ProcessingFee = record.ProcessingFee,
        //            GSTOnProcessingFee = record.GSTOnProcessingFee,
        //            ProcessingFeeReceivedDate = record.ProcessingFeeReceivedDate,
        //            DueDate = record.DueDate,
        //            SettledDate = record.SettledDate,
        //            PurchaseSource = record.PurchaseSource,
        //            InvoiceNumber = record.InvoiceNumber,
        //            InvoiceDate = record.InvoiceDate,
        //            InterestWaiver = record.InterestWaiver,
        //            Status = record.Status,
        //            IsActive = record.Status?.ToLower() != "closed",
        //            CreatedDate = DateTime.UtcNow,

        //            VehicleInfo = new VehicleInfo
        //            {
        //                Make = record.VehicleMake,
        //                Model = record.VehicleModel,
        //                RegistrationNumber = record.VehicleRegistrationNumber,
        //                Year = record.VehicleYear,
        //                ChassisNumber = record.VehicleChassisNumber,
        //                EngineNumber = record.VehicleEngineNumber,
        //                Value = record.VehicleValue
        //            },

        //            LoanPayments = new List<LoanPayment>()
        //        };

        //        // Add Loan Payments
        //        AddPaymentIfExists(record.Payment1Date, record.PaymentRecieved1, record.InterestEarned1, record.DelayedInterestEarned1, record.PrincipalRecieved1, loan);
        //        AddPaymentIfExists(record.Payment2Date, record.PaymentRecieved2, record.InterestEarned2, record.DelayedInterestEarned2, record.PrincipalRecieved2, loan);
        //        AddPaymentIfExists(record.Payment3Date, record.PaymentRecieved3, record.InterestEarned3, record.DelayedInterestEarned3, record.PrincipalRecieved3, loan);

        //        await _loanRepository.AddAsync(loan);

        //        // Create corresponding entry in LoanCalculationService for bulk upload
        //        try
        //        {
        //            var createLoanRequest = new CreateLoanRequestDto
        //            {
        //                CustomerId = dealer.Id,
        //                PrincipalAmount = record.Amount,
        //                InterestRate = record.InterestRate,
        //                ProcessingFeeRate = CalculateProcessingFeeRate(record.ProcessingFee, record.Amount),
        //                GSTPercent = CalculateGSTPercent(record.GSTOnProcessingFee, record.ProcessingFee),
        //                StartDate = record.DateOfWithdraw,
        //                DueDays = 60,
        //                DelayInterestRate = record.DelayedROI,
        //                Status = record.Status
        //            };

        //            var loanDetail = await _loanCalculationService.CreateLoanAsync(createLoanRequest);

        //            loan.LoanDetailId = loanDetail.LoanId;
        //            await _loanRepository.UpdateAsync(loan);

        //            if (record.ProcessingFee > 0)
        //            {
        //                await _loanCalculationService.AddBulkLoanFeeAsync(new BulkLoanFeeDto
        //                {
        //                    LoanId = loanDetail.LoanId,
        //                    FeeAmount = record.ProcessingFee,
        //                    GSTAmount = record.GSTOnProcessingFee ?? 0,
        //                    TotalFeeWithGST = record.ProcessingFee + (record.GSTOnProcessingFee ?? 0)
        //                    //FeeReceivedDate = record.ProcessingFeeReceivedDate
        //                });
        //            }
        //            // Add bulk installments with breakdowns
        //            if (record.Payment1Date.HasValue && record.PaymentRecieved1.HasValue)
        //            {
        //                await _loanCalculationService.AddBulkInstallmentAsync(new BulkInstallmentDto
        //                {
        //                    LoanId = loanDetail.LoanId,
        //                    PaidDate = record.Payment1Date.Value,
        //                    AmountPaid = record.PaymentRecieved1.Value,
        //                    AdjustedToPrincipal = record.PrincipalRecieved1 ?? 0,
        //                    AdjustedToInterest = record.InterestEarned1 ?? 0,
        //                    AdjustedToDelayInterest = record.DelayedInterestEarned1 ?? 0,
        //                    AdjustedToFee = record.ProcessingFee + (record.GSTOnProcessingFee ?? 0),
        //                    DueFee = 0,
        //                    DueInterest = 0
        //                });
        //            }

        //            if (record.Payment2Date.HasValue && record.PaymentRecieved2.HasValue)
        //            {
        //                await _loanCalculationService.AddBulkInstallmentAsync(new BulkInstallmentDto
        //                {
        //                    LoanId = loanDetail.LoanId,
        //                    PaidDate = record.Payment2Date.Value,
        //                    AmountPaid = record.PaymentRecieved2.Value,
        //                    AdjustedToPrincipal = record.PrincipalRecieved2 ?? 0,
        //                    AdjustedToInterest = record.InterestEarned2 ?? 0,
        //                    AdjustedToDelayInterest = record.DelayedInterestEarned2 ?? 0,
        //                    AdjustedToFee = 0,
        //                    DueFee = 0,
        //                    DueInterest = 0
        //                });
        //            }

        //            if (record.Payment3Date.HasValue && record.PaymentRecieved3.HasValue)
        //            {
        //                await _loanCalculationService.AddBulkInstallmentAsync(new BulkInstallmentDto
        //                {
        //                    LoanId = loanDetail.LoanId,
        //                    PaidDate = record.Payment3Date.Value,
        //                    AmountPaid = record.PaymentRecieved3.Value,
        //                    AdjustedToPrincipal = record.PrincipalRecieved3 ?? 0,
        //                    AdjustedToInterest = record.InterestEarned3 ?? 0,
        //                    AdjustedToDelayInterest = record.DelayedInterestEarned3 ?? 0,
        //                    AdjustedToFee = 0,
        //                    DueFee = 0,
        //                    DueInterest = 0
        //                });
        //            }

        //            if (record.InterestWaiver.HasValue
        //                && record.InterestWaiver.Value > 0
        //                && !string.IsNullOrWhiteSpace(record.WaiverType))
        //            {
        //                var waiver = new WaiverDto
        //                {
        //                    LoanId = loanDetail.LoanId,          // << demanded mapping
        //                    InstallmentId = null,                    // bulk load has no instal‑id yet
        //                    WaiverType = record.WaiverType.Trim(),   // “ProcessingFee”, “Interest”, …
        //                    Amount = record.InterestWaiver.Value,
        //                    Reason = "Imported via bulk upload",
        //                    CreatedAt = DateTime.UtcNow
        //                };

        //                await _loanCalculationService.WaiveLoanComponentAsync(loanDetail.LoanId, new WaiverRequestDto
        //                {
        //                    WaiverType = record.WaiverType,
        //                    Amount = record.InterestWaiver.Value,
        //                    Reason = $"Bulk import waiver of type {record.WaiverType}"
        //                });

        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Failed to create loan detail for bulk upload row {row}: {ex.Message}");
        //            // Continue processing other rows
        //        }
        //    }

        //    return true;
        //}


        public async Task<decimal> CalculateInterestTillDateAsync(int loanId, DateTime? tillDate = null)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
                throw new ApplicationException($"Loan with ID {loanId} not found");

            var endDate = tillDate ?? DateTime.UtcNow;
            var days = (endDate - loan.DateOfWithdraw).Days;
            if (days < 0) days = 0;

            // Calculate interest based on daily rate
            var dailyRate = loan.InterestRate / 365;
            var interest = loan.Amount * (decimal)days * dailyRate / 100;

            return Math.Round(interest, 2);
        }

        private static decimal? TryDecimal(string input)
        {
            // Remove commas and other formatting characters before parsing
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var cleanInput = input.Replace(",", "").Replace("₹", "").Trim();
            return decimal.TryParse(cleanInput, out var value) ? value : null;
        }

        private static DateTime? TryDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Try multiple date formats
            string[] formats = { "dd-MM-yyyy", "MM-dd-yyyy", "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy" };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    return date;
            }

            return DateTime.TryParse(input, out var parsedDate) ? parsedDate : null;
        }

        private int? TryInt(string input)
        {
            return int.TryParse(input, out var result) ? result : null;
        }

        private static void AddPaymentIfExists(DateTime? date, decimal? received, decimal? interest, decimal? delayed, decimal? principal, Loan loan)
        {
            if (date.HasValue)
            {
                loan.LoanPayments.Add(new LoanPayment
                {
                    PaymentDate = date.Value,
                    PaymentReceived = received ?? 0,
                    InterestEarned = interest ?? 0,
                    DelayedInterestEarned = delayed ?? 0,
                    PrincipalReceived = principal ?? 0
                });
            }
        }

        private string GenerateLoanNumber()
        {
            // Generate a unique loan number
            return $"LOAN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        // Helper methods to calculate rates and periods
        private decimal CalculateProcessingFeeRate(decimal? processingFee, decimal principalAmount)
        {
            if (!processingFee.HasValue || processingFee <= 0 || principalAmount <= 0)
                return 0;

            return (processingFee.Value / principalAmount) * 100;
        }

        private decimal CalculateGSTPercent(decimal? gstAmount, decimal? processingFee)
        {
            if (!gstAmount.HasValue || !processingFee.HasValue || gstAmount <= 0 || processingFee <= 0)
                return 18; // Default GST rate

            return (gstAmount.Value / processingFee.Value) * 100;
        }

        public async Task<string> GenerateLoanNumberAsync()
        {
            var latestLoan = await _loanRepository
                .GetQueryable()
                .OrderByDescending(l => l.LoanNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (latestLoan != null && !string.IsNullOrEmpty(latestLoan.LoanNumber))
            {
                var numericPart = new string(latestLoan.LoanNumber
                    .SkipWhile(c => !char.IsDigit(c))
                    .ToArray());

                if (int.TryParse(numericPart, out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            return $"TSC{nextNumber:D3}";
        }



    }
}
