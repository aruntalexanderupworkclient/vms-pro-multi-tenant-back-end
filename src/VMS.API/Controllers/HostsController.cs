using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.API.Filters;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Hosts;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/hosts")]
[Authorize]
public class HostsController : ControllerBase
{
    private readonly IHostPersonService _hostPersonService;

    public HostsController(IHostPersonService hostPersonService)
    {
        _hostPersonService = hostPersonService;
    }

    [HttpGet]
    [HasPermission("Hosts", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<HostPersonDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _hostPersonService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Hosts", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<HostPersonDto>), 200)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _hostPersonService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [HasPermission("Hosts", "CanCreate")]
    [ProducesResponseType(typeof(ApiResponse<HostPersonDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateHostPersonDto dto)
    {
        var result = await _hostPersonService.CreateAsync(dto);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Hosts", "CanUpdate")]
    [ProducesResponseType(typeof(ApiResponse<HostPersonDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHostPersonDto dto)
    {
        var result = await _hostPersonService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Hosts", "CanDelete")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _hostPersonService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
