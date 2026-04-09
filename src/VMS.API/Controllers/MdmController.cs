using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Mdm;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/mdm")]
[Authorize]
public class MdmController : ControllerBase
{
    private readonly IMdmService _mdmService;

    public MdmController(IMdmService mdmService)
    {
        _mdmService = mdmService;
    }

    [HttpGet("tenant-types")]
    [ProducesResponseType(typeof(ApiResponse<List<MdmItemDto>>), 200)]
    public async Task<IActionResult> GetTenantTypes()
    {
        var result = await _mdmService.GetTenantTypesAsync();
        return Ok(result);
    }

    [HttpGet("plan-types")]
    [ProducesResponseType(typeof(ApiResponse<List<MdmItemDto>>), 200)]
    public async Task<IActionResult> GetPlanTypes()
    {
        var result = await _mdmService.GetPlanTypesAsync();
        return Ok(result);
    }

    [HttpGet("location-types")]
    [ProducesResponseType(typeof(ApiResponse<List<MdmItemDto>>), 200)]
    public async Task<IActionResult> GetLocationTypes()
    {
        var result = await _mdmService.GetLocationTypesAsync();
        return Ok(result);
    }

    [HttpGet("file-types")]
    [ProducesResponseType(typeof(ApiResponse<List<MdmItemDto>>), 200)]
    public async Task<IActionResult> GetFileTypes()
    {
        var result = await _mdmService.GetFileTypesAsync();
        return Ok(result);
    }

    [HttpGet("entity-types")]
    [ProducesResponseType(typeof(ApiResponse<List<MdmItemDto>>), 200)]
    public async Task<IActionResult> GetEntityTypes()
    {
        var result = await _mdmService.GetEntityTypesAsync();
        return Ok(result);
    }
}
