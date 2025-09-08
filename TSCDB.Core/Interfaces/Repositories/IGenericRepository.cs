using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TscLoanManagement.TSCDB.Core.Domain.DDR;

namespace TscLoanManagement.TSCDB.Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAllAsync(Func<IQueryable<T>, IIncludableQueryable<T, object>> include);

        Task InsertAsync(T entity);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
                                   Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        IQueryable<T> GetQueryable();

    }
}
