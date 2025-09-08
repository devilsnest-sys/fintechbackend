using TscLoanManagement.TSCDB.Application.DTOs;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(int id);
        Task CreateRoleAsync(CreateRoleDto dto);
        Task UpdateRoleAsync(int id, RoleDto dto);
        Task DeleteRoleAsync(int id);

        Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
        Task AddPermissionAsync(PermissionDto dto);

    }
}
