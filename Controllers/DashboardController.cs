using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.Interfaces;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDealerDashboardSummary([FromQuery] string dealerId)
        {
            if (string.IsNullOrEmpty(dealerId))
                return BadRequest("DealerId is required.");

            var result = await _dashboardService.GetSummaryAsync(dealerId);
            return Ok(result);
        }
    }
}
