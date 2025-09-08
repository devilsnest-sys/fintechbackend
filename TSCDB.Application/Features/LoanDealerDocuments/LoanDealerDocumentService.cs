using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TscLoanManagement.TSCDB.Application.DTOs.LoanDocuments;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.LoanDocuments;
using TscLoanManagement.TSCDB.Infrastructure.Data.Context;

namespace TscLoanManagement.TSCDB.Application.Features.LoanDealerDocuments
{
    public class LoanDealerDocumentService : ILoanDealerDocumentService
    {
        private readonly TSCDbContext _context;
        private readonly IMapper _mapper;

        public LoanDealerDocumentService(TSCDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddLoanDocumentAsync(LoanDocumentMasterDto dto)
        {
            var entity = _mapper.Map<LoanDocumentMaster>(dto);
            await _context.LoanDocumentMasters.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LoanDocumentMasterDto>> GetAllAsync()
        {
            var data = await _context.LoanDocumentMasters
                .Include(x => x.Activities)
                .ToListAsync();
            return _mapper.Map<List<LoanDocumentMasterDto>>(data);
        }

        public async Task<LoanDocumentMasterDto> GetByLoanNumberAsync(string loanNumber)
        {
            var data = await _context.LoanDocumentMasters
                .Include(x => x.Activities)
                .FirstOrDefaultAsync(x => x.LoanNumber == loanNumber);
            return _mapper.Map<LoanDocumentMasterDto>(data);
        }

        public async Task AddActivityAsync(string loanNumber, LoanDocumentActivityDto dto)
        {
            var master = await _context.LoanDocumentMasters.FirstOrDefaultAsync(x => x.LoanNumber == loanNumber);
            if (master == null) throw new Exception("Loan not found");

            var activity = _mapper.Map<LoanDocumentActivity>(dto);
            activity.LoanDocumentMaster = master;

            await _context.LoanDocumentActivities.AddAsync(activity);
            await _context.SaveChangesAsync();
        }

        public async Task BulkUploadAsync(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    // Read DealerCode from Excel
                    string dealerCode = worksheet.Cells[row, 2].Text.Trim();

                    // Find Dealer by DealerCode
                    var dealer = await _context.Dealers
                        .FirstOrDefaultAsync(d => d.DealerCode == dealerCode);

                    if (dealer == null)
                    {
                        //_logger.LogWarning($"Dealer not found for code: {dealerCode} (row {row})");
                        continue; // Skip row if dealer doesn't exist
                    }

                    // Create DTO with correct DealerId
                    var dto = new LoanDocumentMasterDto
                    {
                        DealerId = dealer.Id,
                        DealerCode = dealerCode,
                        LoanNumber = worksheet.Cells[row, 3].Text,
                        VehicleNumber = worksheet.Cells[row, 4].Text,
                        Location = worksheet.Cells[row, 5].Text,
                        RelationshipManager = worksheet.Cells[row, 6].Text,
                        MakeModel = worksheet.Cells[row, 7].Text,
                        ProcuredFrom = worksheet.Cells[row, 8].Text,
                        VendorName = worksheet.Cells[row, 9].Text,
                        Status = worksheet.Cells[row, 10].Text,
                        DateOfDisbursement = TryParseDate(worksheet.Cells[row, 11].Text),
                        Tenure = TryParseInt(worksheet.Cells[row, 12].Text),
                        DocumentStatus = worksheet.Cells[row, 13].Text,
                        Remarks = worksheet.Cells[row, 14].Text,
                        Activities = new List<LoanDocumentActivityDto>()
                    };

                    // Optional activity: Document Received
                    var docReceivedDate = TryParseDate(worksheet.Cells[row, 15].Text);
                    if (docReceivedDate != null)
                    {
                        dto.Activities.Add(new LoanDocumentActivityDto
                        {
                            ActivityType = "DocumentReceived",
                            FromPerson = worksheet.Cells[row, 16].Text,
                            ToPerson = worksheet.Cells[row, 17].Text,
                            Date = docReceivedDate.Value
                        });
                    }

                    // Additional activities like Handover, RTD, etc., can be added similarly.

                    // Save
                    var entity = _mapper.Map<LoanDocumentMaster>(dto);
                    await _context.LoanDocumentMasters.AddAsync(entity);
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, $"Error processing row {row}");
                }
            }

            await _context.SaveChangesAsync();
        }


        private static DateTime? TryParseDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            var formats = new[] { "dd-MM-yyyy", "d-M-yyyy", "dd/MM/yyyy" }; // handle common variations
            return DateTime.TryParseExact(
                input.Trim(),
                formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var result)
                ? result
                : null;
        }


        private static int TryParseInt(string input)
        {
            return int.TryParse(input, out var result) ? result : 0;
        }

    }

}
