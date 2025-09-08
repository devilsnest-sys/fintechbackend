using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _service;

        public RolePermissionController(IRolePermissionService service)
        {
            _service = service;
        }

        // ===== ROLES =====
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _service.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var role = await _service.GetRoleByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            await _service.CreateRoleAsync(dto);
            return Ok(new { message = "Role created" });
        }

        [HttpPatch("roles/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleDto dto)
        {
            await _service.UpdateRoleAsync(id, dto);
            return Ok(new { message = "Role updated" });
        }

        //[HttpDelete("roles/{id}")]
        //public async Task<IActionResult> DeleteRole(int id)
        //{
        //    await _service.DeleteRoleAsync(id);
        //    return Ok(new { message = "Role deleted" });
        //}

        // ===== PERMISSIONS =====
        [HttpGet("permissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await _service.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        [HttpPost("permissions")]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionDto dto)
        {
            await _service.AddPermissionAsync(dto);
            return Ok(new { message = "Permission created successfully" });
        }


    }
}
