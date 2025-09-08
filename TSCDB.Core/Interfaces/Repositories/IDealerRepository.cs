using System.Collections.Generic;
using System.Threading.Tasks;
using TscLoanManagement.TSCDB.Core.Domain.Dealer;

namespace TscLoanManagement.TSCDB.Core.Interfaces.Repositories
{
    public interface IDealerRepository
    {
        Task<IEnumerable<Dealer>> GetAllAsync();
        Task<Dealer> GetByIdAsync(int id);
        Task<Dealer> GetDealerByUserIdAsync(string userId);
        Task AddAsync(Dealer dealer);
        Task UpdateAsync(Dealer dealer);
        Task DeleteAsync(Dealer dealer);
        Task BulkInsertAsync(IEnumerable<Dealer> dealers);
        Task<Dealer> GetByDealerCodeAsync(string dealerCode);
        Task<List<Dealer>> GetByDealerCodesAsync(List<string> dealerCodes);
        IQueryable<Dealer> GetQueryable();
        Task<Dealer> GetByEmailAsync(string email);
        Task<Dealer> GetByPhoneAsync(string phoneNumber);

    }
}