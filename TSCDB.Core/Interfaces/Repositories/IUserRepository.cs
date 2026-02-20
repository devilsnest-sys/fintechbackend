using TscLoanManagement.TSCDB.Core.Domain.Authentication;

namespace TscLoanManagement.TSCDB.Core.Interfaces.Repositories
{
    // OOP (Abstraction): user-specific persistence contract.
    // SOLID (ISP): extends generic repository with only user-focused operations.
    // Implemented by: TSCDB.Infrastructure.Repositories.UserRepository.
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<int> SaveChangesAsync();
    }
}
