using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TscLoanManagement.TSCDB.Core.Domain.Dealer;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;
using TscLoanManagement.TSCDB.Infrastructure.Data.Context;

namespace TscLoanManagement.TSCDB.Infrastructure.Repositories
{
    // SOLID (SRP): Dealer-specific data access and aggregate loading.
    // SOLID (DIP): Consumed through IDealerRepository abstraction.
    // Depends on: TSCDB.Infrastructure.Data.Context.TSCDbContext and Dealer domain model.
    public class DealerRepository : IDealerRepository
    {
        private readonly TSCDbContext _context;

        public DealerRepository(TSCDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dealer>> GetAllAsync()
        {
            return await _context.Dealers
                .Include(d => d.BorrowerDetails)
                .Include(d => d.GuarantorDetails)
                .Include(d => d.ChequeDetails)
                .Include(d => d.SecurityDepositDetails)
                .Include(d => d.DocumentUploads)
                .ToListAsync();
        }

        public async Task<Dealer> GetByIdAsync(int id)
        {
            return await _context.Dealers
                .Include(d => d.BorrowerDetails)
                .Include(d => d.GuarantorDetails)
                .Include(d => d.ChequeDetails)
                .Include(d => d.SecurityDepositDetails)
                .Include(d => d.DocumentUploads)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Dealer> GetDealerByUserIdAsync(string userId)
        {
            if (!int.TryParse(userId, out int userIdInt))
            {
                // Optionally handle the case where parsing fails (e.g., return null or throw an exception)
                return null;
            }

            return await _context.Dealers
                .Include(d => d.BorrowerDetails)
                .Include(d => d.GuarantorDetails)
                .Include(d => d.ChequeDetails)
                .Include(d => d.SecurityDepositDetails)
                .Include(d => d.DocumentUploads)
                .FirstOrDefaultAsync(d => d.UserId == userIdInt);
        }


        public async Task AddAsync(Dealer dealer)
        {
            await _context.Dealers.AddAsync(dealer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Dealer dealer)
        {
            _context.Dealers.Update(dealer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Dealer dealer)
        {
            _context.Dealers.Remove(dealer);
            await _context.SaveChangesAsync();
        }

        public async Task BulkInsertAsync(IEnumerable<Dealer> dealers)
        {
            //foreach (var dealer in dealers)
            //{
            //    dealer.UserId = 3; // Ensure FK constraint is satisfied
            //}
            await _context.Dealers.AddRangeAsync(dealers);
            await _context.SaveChangesAsync();
        }

        public async Task<Dealer> GetByDealerCodeAsync(string dealerCode)
        {
            return await _context.Set<Dealer>()
                .FirstOrDefaultAsync(d => d.DealerCode == dealerCode && d.IsActive == true);
        }
        public IQueryable<Dealer> GetQueryable()
        {
            return _context.Dealers.AsNoTracking();
        }

        public async Task<Dealer> GetByEmailAsync(string email)
        {
            return await _context.Dealers.FirstOrDefaultAsync(d => d.EmailId.ToLower() == email.ToLower());
        }

        public async Task<Dealer> GetByPhoneAsync(string phoneNumber)
        {
            return await _context.Dealers.FirstOrDefaultAsync(d => d.ContactNo == phoneNumber);
        }


        public async Task<List<Dealer>> GetByDealerCodesAsync(List<string> dealerCodes)
        {
            return await _context.Dealers
                .Where(d => dealerCodes.Contains(d.DealerCode))
                .ToListAsync();
        }
    }
}
