using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;
using TscLoanManagement.TSCDB.Infrastructure.Data.Context;

namespace TscLoanManagement.TSCDB.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly TSCDbContext _context;

        public DashboardRepository(TSCDbContext context)
        {
            _context = context;
        }

        //public async Task<List<DealerDashboardSummaryDto>> GetDealerDashboardSummaryAsync(string dealerId)
        //{
        //    var result = new List<DealerDashboardSummaryDto>();

        //    using var command = _context.Database.GetDbConnection().CreateCommand();
        //    command.CommandText = "EXEC GetDealerDashboardSummaryUsp @DealerId";
        //    command.Parameters.Add(new SqlParameter("@DealerId", dealerId));
        //    await _context.Database.OpenConnectionAsync();

        //    using var reader = await command.ExecuteReaderAsync();
        //    while (await reader.ReadAsync())
        //    {
        //        result.Add(new DealerDashboardSummaryDto
        //        {
        //            DealerId = reader["DealerId"] != DBNull.Value ? Convert.ToInt32(reader["DealerId"]) : (int?)null,
        //            DealershipName = reader["DealershipName"].ToString(),
        //            TotalSanctionLimit = reader["TotalSanctionLimit"] != DBNull.Value ? Convert.ToDecimal(reader["TotalSanctionLimit"]) : 0,
        //            TotalLoanAmount = reader["TotalLoanAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalLoanAmount"]) : 0,
        //            TotalPrincipalPaid = reader["TotalPrincipalPaid"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPrincipalPaid"]) : 0,
        //            AvailableLimit = reader["AvailableLimit"] != DBNull.Value ? Convert.ToDecimal(reader["AvailableLimit"]) : 0,
        //            TotalWaiversAmount = reader["TotalWaiversAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalWaiversAmount"]) : 0,
        //            TotalAmountPending = reader["TotalAmountPending"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmountPending"]) : 0
        //        });
        //    }

        //    return result;
        //}
        public async Task<DealerDashboardSummaryDto> GetDealerDashboardSummaryAsync(string dealerId)
        {
            var summaries = await _context.DealerDashboardSummary
                .FromSqlRaw("EXEC GetDealerDashboardSummaryUsptest @DealerId", new SqlParameter("@DealerId", dealerId))
                .ToListAsync();

            // Return the first or default (since you're expecting one object)
            return summaries.FirstOrDefault();
        }


    }
}
