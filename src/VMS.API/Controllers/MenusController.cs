using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.API.Filters;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Menus;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/menus")]
[Authorize]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<MenuDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _menuService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("tree")]
    [ProducesResponseType(typeof(ApiResponse<List<MenuDto>>), 200)]
    public async Task<IActionResult> GetTree()
    {
        var result = await _menuService.GetTreeAsync();
        return Ok(result);
    }

    [HttpGet("my-menus")]
    [ProducesResponseType(typeof(ApiResponse<List<MenuWithPermissionDto>>), 200)]
    public async Task<IActionResult> GetMyMenus()
    {
        var result = await _menuService.GetMyMenusAsync();
        return Ok(result);
    }

    [HttpGet("by-role/{roleId:guid}")]
    [HasPermission("Roles", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<List<MenuWithPermissionDto>>), 200)]
    public async Task<IActionResult> GetByRole(Guid roleId)
    {
        var result = await _menuService.GetMenusByRoleAsync(roleId);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission("Settings", "CanCreate")]
    [ProducesResponseType(typeof(ApiResponse<MenuDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateMenuDto dto)
    {
        var result = await _menuService.CreateAsync(dto);
        return result.Success ? CreatedAtAction(nameof(GetAll), result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Settings", "CanUpdate")]
    [ProducesResponseType(typeof(ApiResponse<MenuDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuDto dto)
    {
        var result = await _menuService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Settings", "CanDelete")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _menuService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
