using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VMS.API.Filters;
using VMS.API.Hubs;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Visitors;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/visitors")]
[Authorize]
public class VisitorsController : ControllerBase
{
    private readonly IVisitorService _visitorService;
    private readonly IVisitService _visitService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public VisitorsController(IVisitorService visitorService, IVisitService visitService, IHubContext<NotificationHub> hubContext)
    {
        _visitorService = visitorService;
        _visitService = visitService;
        _hubContext = hubContext;
    }

    [HttpGet]
    [HasPermission("Visitors", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<VisitorDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _visitorService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Visitors", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<VisitorDto>), 200)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _visitorService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [HasPermission("Visitors", "CanCreate")]
    [ProducesResponseType(typeof(ApiResponse<VisitorDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateVisitorDto dto)
    {
        var result = await _visitorService.CreateAsync(dto);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Visitors", "CanUpdate")]
    [ProducesResponseType(typeof(ApiResponse<VisitorDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVisitorDto dto)
    {
        var result = await _visitorService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Visitors", "CanDelete")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _visitorService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // === Visits ===

    [HttpGet("visits")]
    [HasPermission("Visitors", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<VisitDto>>), 200)]
    public async Task<IActionResult> GetVisits([FromQuery] Guid? visitorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _visitService.GetAllAsync(visitorId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("visits/{id:guid}")]
    [HasPermission("Visitors", "CanRead")]
    [ProducesResponseType(typeof(ApiResponse<VisitDto>), 200)]
    public async Task<IActionResult> GetVisitById(Guid id)
    {
        var result = await _visitService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("visits")]
    [HasPermission("Visitors", "CanCreate")]
    [ProducesResponseType(typeof(ApiResponse<VisitDto>), 201)]
    public async Task<IActionResult> CreateVisit([FromBody] CreateVisitDto dto)
    {
        var result = await _visitService.CreateAsync(dto);
        return result.Success ? CreatedAtAction(nameof(GetVisitById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("visits/{id:guid}/checkin")]
    [HasPermission("Visitors", "CanUpdate")]
    [ProducesResponseType(typeof(ApiResponse<VisitDto>), 200)]
    public async Task<IActionResult> CheckIn(Guid id, [FromBody] CheckInDto dto)
    {
        var result = await _visitService.CheckInAsync(id, dto);

        if (result.Success && result.Data != null)
        {
            await _hubContext.Clients.Group($"tenant_{User.FindFirst("tenantId")?.Value}")
                .SendAsync("VisitorCheckedIn", new
                {
                    result.Data.Id,
                    result.Data.VisitorName,
                    result.Data.HostName,
                    result.Data.LocationName,
                    result.Data.CheckInTime
                });
        }

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("visits/{id:guid}/checkout")]
    [HasPermission("Visitors", "CanUpdate")]
    [ProducesResponseType(typeof(ApiResponse<VisitDto>), 200)]
    public async Task<IActionResult> CheckOut(Guid id, [FromBody] CheckOutDto dto)
    {
        var result = await _visitService.CheckOutAsync(id, dto);

        if (result.Success && result.Data != null)
        {
            await _hubContext.Clients.Group($"tenant_{User.FindFirst("tenantId")?.Value}")
                .SendAsync("VisitorCheckedOut", new
                {
                    result.Data.Id,
                    result.Data.VisitorName,
                    result.Data.CheckOutTime
                });
        }

        return result.Success ? Ok(result) : BadRequest(result);
    }
}
