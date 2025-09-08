using TscLoanManagement.TSCDB.Core.Domain.Authentication;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
