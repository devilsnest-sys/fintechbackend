using TscLoanManagement.TSCDB.Core.Domain.Authentication;

namespace TscLoanManagement.TSCDB.Core.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllWithPermissionsAsync();
        Task<Role> GetByIdWithPermissionsAsync(int id);
        Task<Role> GetByIdAsync(int id);
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
        Task<Role> GetByNameAsync(string name);

    }
}
