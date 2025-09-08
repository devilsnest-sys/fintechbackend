using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ReportsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("download")]
        public async Task<IActionResult> DownloadReport([FromBody] ReportRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ReportType))
                return BadRequest("ReportType is required.");

            var dt = new DataTable();

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (var cmd = new SqlCommand("SP_UnifiedLoanReports", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ReportType", request.ReportType);
                cmd.Parameters.AddWithValue("@StartDate", request.StartDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", request.EndDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DealerCode", DBNull.Value); // Always null

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(dt, $"{request.ReportType}_Report");
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"{request.ReportType}_Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }
    }
}
