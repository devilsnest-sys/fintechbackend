using AutoMapper;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Authentication;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Authentication
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRoleRepository _roleRepo;
        private readonly IGenericRepository<RolePermission> _rolePermissionRepo;
        private readonly IGenericRepository<Permission> _permissionRepo;
        private readonly IMapper _mapper;

        public RolePermissionService(
            IRoleRepository roleRepo,
            IGenericRepository<RolePermission> rolePermissionRepo,
            IGenericRepository<Permission> permissionRepo,
            IMapper mapper)
        {
            _roleRepo = roleRepo;
            _rolePermissionRepo = rolePermissionRepo;
            _permissionRepo = permissionRepo;
            _mapper = mapper;
        }

        // ========== Roles ==========
        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepo.GetAllWithPermissionsAsync();

            return roles.Select(role => new RoleDto
            {
                Id = (int)role.Id,
                Name = role.Name,
                Permissions = role.RolePermissions?
                    .Where(rp => rp.Permission != null) // Add null check
                    .Select(rp => new PermissionDto
                    {
                        Id = (int)rp.Permission.Id,
                        Name = rp.Permission.Name,
                        Description = rp.Permission.Description,
                        category = rp.Permission.category
                    }).ToList() ?? new List<PermissionDto>()
            }).ToList();
        }

        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepo.GetByIdWithPermissionsAsync(id);
            if (role == null) return null;

            return new RoleDto
            {
                Id = (int)role.Id,
                Name = role.Name,
                Permissions = role.RolePermissions?
                    .Where(rp => rp.Permission != null) // Add null check
                    .Select(rp => new PermissionDto
                    {
                        Id = (int)rp.Permission.Id,
                        Name = rp.Permission.Name,
                        Description = rp.Permission.Description,
                        category = rp.Permission.category
                    }).ToList() ?? new List<PermissionDto>()
            };
        }

        public async Task CreateRoleAsync(CreateRoleDto dto)
        {
            var role = new Role { Name = dto.Name };
            await _roleRepo.AddAsync(role);

            if (dto.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase)) 
            { 
                var allPermissions = await _permissionRepo.GetAllAsync();
                foreach (var permission in allPermissions) 
                {
                    await _rolePermissionRepo.AddAsync(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permission.Id,

                    });
                }
            }
        }

        public async Task UpdateRoleAsync(int id, RoleDto dto)
        {
            var role = await _roleRepo.GetByIdAsync(id);
            if (role == null) return;

            // Update name only if a new (non-null and changed) value is provided
            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != role.Name)
            {
                role.Name = dto.Name;
                await _roleRepo.UpdateAsync(role);
            }

            // Only update permissions if a new list is explicitly provided
            if (dto.Permissions != null)
            {
                // Delete existing permissions
                var existing = await _rolePermissionRepo.GetAllAsync(x => x.RoleId == id);
                foreach (var item in existing)
                    await _rolePermissionRepo.DeleteAsync(item);

                if (role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    var allPermissions = await _permissionRepo.GetAllAsync();
                    foreach (var perm in allPermissions)
                    {
                        await _rolePermissionRepo.AddAsync(new RolePermission
                        {
                            RoleId = id,
                            PermissionId = perm.Id
                        });
                    }
                }

                // Add new permissions
                foreach (var perm in dto.Permissions)
                {
                    await _rolePermissionRepo.AddAsync(new RolePermission
                    {
                        RoleId = id,
                        PermissionId = perm.Id
                    });
                }
            }
        }


        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleRepo.GetByIdAsync(id);
            if (role != null)
                await _roleRepo.DeleteAsync(role);
        }

        // ========== Permissions ==========
        public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }

        public async Task AddPermissionAsync(PermissionDto dto)
        {
            var permission = new Permission
            {
                Name = dto.Name,
                Description = dto.Description,
                category = dto.category
            };
            await _permissionRepo.AddAsync(permission);

            var adminRole = await _roleRepo.GetByNameAsync("Admin");
            if (adminRole != null)
            {
                await _rolePermissionRepo.AddAsync(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
            }

        }
    }
}