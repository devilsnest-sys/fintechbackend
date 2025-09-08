using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
//using OfficeOpenXml.License;

namespace TscLoanManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DealersController : ControllerBase
    {
        private readonly IDealerService _dealerService;
        private readonly IMapper _mapper;

        public DealersController(IDealerService dealerService, IMapper mapper)
        {
            _dealerService = dealerService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all dealers
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DealerDto>>> GetAllDealers()
        {
            try
            {
                var dealers = await _dealerService.GetAllDealersAsync();
                return Ok(dealers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Gets a dealer by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DealerDto>> GetDealerById(int id)
        {
            try
            {
                var dealer = await _dealerService.GetDealerByIdAsync(id);
                if (dealer == null)
                    return NotFound(new { message = $"Dealer with ID {id} not found" });

                return Ok(dealer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Gets a dealer by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<DealerDto>> GetDealerByUserId(string userId)
        {
            try
            {
                var dealer = await _dealerService.GetDealerByUserIdAsync(userId);
                if (dealer == null)
                    return NotFound(new { message = $"Dealer with User ID {userId} not found" });

                return Ok(dealer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Creates a new dealer
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<DealerDto>> CreateDealer([FromBody] DealerDto dealerDto)
        {
            try
            {
                if (dealerDto == null)
                    return BadRequest(new { message = "Dealer data is required" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdDealer = await _dealerService.CreateDealerAsync(dealerDto);
                return CreatedAtAction(nameof(GetDealerById), new { id = createdDealer.Id }, createdDealer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.InnerException?.Message}" });
            }
        }

        /// <summary>
        /// Updates an existing dealer
        /// </summary>
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> UpdateDealer(int id, [FromBody] DealerDto dealerDto)
        {
            try
            {
                if (dealerDto == null)
                    return BadRequest(new { message = "Dealer data is required" });

                if (id != dealerDto.Id)
                    return BadRequest(new { message = "ID mismatch between route and dealer data" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _dealerService.UpdateDealerAsync(dealerDto);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Soft deletes a dealer by marking IsActive as false
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteDealer(int id)
        {
            try
            {
                await _dealerService.DeleteDealerAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Registers a new dealer - alternative endpoint for dealer creation
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<DealerDto>> RegisterDealer([FromBody] DealerDto dealerDto)
        {
            try
            {
                if (dealerDto == null)
                    return BadRequest(new { message = "Dealer data is required" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdDealer = await _dealerService.CreateDealerAsync(dealerDto);
                return CreatedAtAction(nameof(GetDealerById), new { id = createdDealer.Id }, createdDealer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Updates a dealer's status
        /// </summary>
        [HttpPut("UpdateDealerStatus")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateDealerStatus([FromBody] UpdateDealerStatusDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Status update data is required" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _dealerService.UpdateDealerStatusAsync(dto);
                return Ok(new { success = result, message = "Dealer status updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost("bulk-upload")]
        [AllowAnonymous]
        public async Task<ActionResult<BulkUploadResultDto>> BulkUploadDealers([FromForm] DealerBulkUploadDto uploadDto)
        {
            try
            {
                if (uploadDto == null || uploadDto.ExcelFile == null)
                    return BadRequest(new { message = "Excel file is required" });

                // Check file extension
                var fileExtension = Path.GetExtension(uploadDto.ExcelFile.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                    return BadRequest(new { message = "Only Excel files (.xlsx or .xls) are allowed" });

                // Check file size (10 MB limit)
                if (uploadDto.ExcelFile.Length > 10 * 1024 * 1024)
                    return BadRequest(new { message = "File size exceeds the limit of 10 MB" });

                var result = await _dealerService.BulkUploadDealersAsync(uploadDto.ExcelFile);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                throw new Exception("Internal server error: " + inner, ex);
            }

        }

        //[HttpGet("download-template")]
        //[AllowAnonymous]
        //public async Task<IActionResult> DownloadTemplate()
        //{
        //    try
        //    {
        //        // Set the EPPlus license mode
        //        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        //        using (var package = new ExcelPackage())
        //        {
        //            var worksheet = package.Workbook.Worksheets.Add("Dealers");

        //            // Headers (first row)
        //            var headers = new string[]
        //            {
        //                "DealerCode", "LoanProposalNo", "DealershipName", "DealershipPAN", "GSTNo",
        //                "GSTRegStatus", "MSMERegistrationNo", "MSMEType", "MSMEStatus", "BusinessCategory",
        //                "BusinessType", "Entity", "ContactNo", "AlternativeContactNo", "EmailId",
        //                "AlternativeEmailId", "ShopAddress", "ParkingYardAddress", "City", "District",
        //                "State", "Pincode", "OfficeStatus", "AgreementDate", "AgreementExpiryDate",
        //                "ParkingStatus", "ParkingAgreementDate", "ParkingAgreementExpiryDate", "DateOfIncorporation",
        //                "DateOfFacilityAgreement", "CIBILOfEntity", "TotalSanctionLimit", "ROI", "ROIPerLakh",
        //                "DelayROI", "ProcessingFee", "ProcessingCharge", "GSTOnProcessingCharge",
        //                "DocumentationCharge", "GSTOnDocCharges", "RelationshipManager", "Status"
        //            };

        //            // Format headers
        //            for (int i = 0; i < headers.Length; i++)
        //            {
        //                worksheet.Cells[1, i + 1].Value = headers[i];
        //                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
        //                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        //            }

        //            // Add a sample row
        //            worksheet.Cells[2, 1].Value = "D12345";
        //            worksheet.Cells[2, 2].Value = "LP123456";
        //            worksheet.Cells[2, 3].Value = "ABC Motors";
        //            worksheet.Cells[2, 4].Value = "ABCPJ1234Q";
        //            worksheet.Cells[2, 5].Value = "27ABCDE1234F1Z5";
        //            worksheet.Cells[2, 6].Value = "Registered";
        //            worksheet.Cells[2, 7].Value = "MSME123456";
        //            worksheet.Cells[2, 8].Value = "Small";
        //            worksheet.Cells[2, 9].Value = "Active";
        //            worksheet.Cells[2, 10].Value = "Automobile";
        //            worksheet.Cells[2, 11].Value = "Retail";
        //            worksheet.Cells[2, 12].Value = "Proprietorship";
        //            worksheet.Cells[2, 13].Value = "9876543210";
        //            worksheet.Cells[2, 14].Value = "8765432109";
        //            worksheet.Cells[2, 15].Value = "contact@abc.com";
        //            worksheet.Cells[2, 16].Value = "info@abc.com";
        //            worksheet.Cells[2, 17].Value = "123 Main Street";
        //            worksheet.Cells[2, 18].Value = "456 Storage Yard";
        //            worksheet.Cells[2, 19].Value = "Mumbai";
        //            worksheet.Cells[2, 20].Value = "Mumbai";
        //            worksheet.Cells[2, 21].Value = "Maharashtra";
        //            worksheet.Cells[2, 22].Value = "400001";
        //            worksheet.Cells[2, 23].Value = "Owned";
        //            worksheet.Cells[2, 24].Value = DateTime.Now.ToString("yyyy-MM-dd");
        //            worksheet.Cells[2, 25].Value = DateTime.Now.AddYears(5).ToString("yyyy-MM-dd");
        //            worksheet.Cells[2, 26].Value = "Rented";
        //            worksheet.Cells[2, 27].Value = DateTime.Now.ToString("yyyy-MM-dd");
        //            worksheet.Cells[2, 28].Value = DateTime.Now.AddYears(3).ToString("yyyy-MM-dd");
        //            worksheet.Cells[2, 29].Value = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd");
        //            worksheet.Cells[2, 30].Value = DateTime.Now.ToString("yyyy-MM-dd");
        //            worksheet.Cells[2, 31].Value = "750";
        //            worksheet.Cells[2, 32].Value = "5000000";
        //            worksheet.Cells[2, 33].Value = "12.5";
        //            worksheet.Cells[2, 34].Value = "1250";
        //            worksheet.Cells[2, 35].Value = "15.5";
        //            worksheet.Cells[2, 36].Value = "1.5";
        //            worksheet.Cells[2, 37].Value = "75000";
        //            worksheet.Cells[2, 38].Value = "13500";
        //            worksheet.Cells[2, 39].Value = "25000";
        //            worksheet.Cells[2, 40].Value = "4500";
        //            worksheet.Cells[2, 41].Value = "Jane Doe";
        //            worksheet.Cells[2, 42].Value = "Pending";

        //            // Format date cells
        //            var dateCells = new int[] { 24, 25, 27, 28, 29, 30 };
        //            foreach (var col in dateCells)
        //            {
        //                worksheet.Cells[2, col].Style.Numberformat.Format = "yyyy-mm-dd";
        //            }

        //            // Format decimal cells
        //            var decimalCells = new int[] { 32, 33, 34, 35, 36, 37, 38, 39, 40 };
        //            foreach (var col in decimalCells)
        //            {
        //                worksheet.Cells[2, col].Style.Numberformat.Format = "#,##0.00";
        //            }

        //            // Auto-fit columns
        //            worksheet.Cells.AutoFitColumns();

        //            // Generate file
        //            var stream = new MemoryStream();
        //            package.SaveAs(stream);
        //            stream.Position = 0;

        //            return File(
        //                stream,
        //                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                "DealerBulkUploadTemplate.xlsx"
        //            );
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
        //    }
        //}

        [HttpGet("generate-codes")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateCodes()
        {
            try
            {
                // Call your service methods to generate codes
                var dealerCode = await _dealerService.GenerateDealerCodeAsync();
                var loanProposalNo = await _dealerService.GenerateLoanProposalNoAsync();

                return Ok(new
                {
                    DealerCode = dealerCode,
                    LoanProposalNo = loanProposalNo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error generating codes: " + ex.Message);
            }
        }
    }
}