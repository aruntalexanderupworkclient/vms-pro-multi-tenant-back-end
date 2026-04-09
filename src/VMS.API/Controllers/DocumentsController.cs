using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Documents;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DocumentDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? entityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _documentService.GetAllAsync(entityId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentDto>), 200)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _documentService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var result = await _documentService.DownloadAsync(id);
        if (result == null)
            return NotFound(ApiResponse.FailResponse("Document not found.", "DOC_NOT_FOUND"));

        return File(result.Value.Data, result.Value.ContentType, result.Value.FileName);
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(ApiResponse<DocumentDto>), 201)]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] UploadDocumentDto dto)
    {
        var result = await _documentService.UploadAsync(file, dto);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _documentService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
