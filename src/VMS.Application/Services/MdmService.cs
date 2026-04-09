using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Mdm;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Interfaces;

namespace VMS.Application.Services;

public class MdmService : IMdmService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public MdmService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<MdmItemDto>>> GetTenantTypesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _uow.Repository<MdmTenantType>().Query()
            .Where(e => e.IsActive)
            .OrderBy(e => e.DisplayOrder)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<MdmItemDto>>.SuccessResponse(_mapper.Map<List<MdmItemDto>>(items));
    }

    public async Task<ApiResponse<List<MdmItemDto>>> GetPlanTypesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _uow.Repository<MdmPlanType>().Query()
            .Where(e => e.IsActive)
            .OrderBy(e => e.DisplayOrder)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<MdmItemDto>>.SuccessResponse(_mapper.Map<List<MdmItemDto>>(items));
    }

    public async Task<ApiResponse<List<MdmItemDto>>> GetLocationTypesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _uow.Repository<MdmLocationType>().Query()
            .Where(e => e.IsActive)
            .OrderBy(e => e.DisplayOrder)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<MdmItemDto>>.SuccessResponse(_mapper.Map<List<MdmItemDto>>(items));
    }

    public async Task<ApiResponse<List<MdmItemDto>>> GetFileTypesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _uow.Repository<MdmFileType>().Query()
            .Where(e => e.IsActive)
            .OrderBy(e => e.DisplayOrder)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<MdmItemDto>>.SuccessResponse(_mapper.Map<List<MdmItemDto>>(items));
    }

    public async Task<ApiResponse<List<MdmItemDto>>> GetEntityTypesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _uow.Repository<MdmEntityType>().Query()
            .Where(e => e.IsActive)
            .OrderBy(e => e.DisplayOrder)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<MdmItemDto>>.SuccessResponse(_mapper.Map<List<MdmItemDto>>(items));
    }
}
