using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.API.Filters;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Roles;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [HasPermission("Roles", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _roleService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Roles", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), 200)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _roleService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [HasPermission("Roles", "CanCreate")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        var result = await _roleService.CreateAsync(dto);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Roles", "CanUpdate")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDto dto)
    {
        var result = await _roleService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Roles", "CanDelete")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _roleService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/permissions")]
    [HasPermission("Roles", "CanUpdate")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> SetPermissions(Guid id, [FromBody] SetRolePermissionsDto dto)
    {
        var result = await _roleService.SetPermissionsAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
