using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.API.Filters;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Locations;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/locations")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    [HasPermission("Locations", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<List<LocationDto>>), 200)]
    public async Task<IActionResult> GetTree()
    {
        var result = await _locationService.GetTreeAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Locations", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<LocationDto>), 200)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _locationService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [HasPermission("Locations", "CanCreate")]
    [ProducesResponseType(typeof(ApiResponse<LocationDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateLocationDto dto)
    {
        var result = await _locationService.CreateAsync(dto);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Locations", "CanUpdate")]
    [ProducesResponseType(typeof(ApiResponse<LocationDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationDto dto)
    {
        var result = await _locationService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Locations", "CanDelete")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _locationService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
