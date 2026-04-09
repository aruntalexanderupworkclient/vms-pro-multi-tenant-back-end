using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Locations;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Interfaces;

namespace VMS.Application.Services;

public class LocationService : ILocationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public LocationService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<LocationDto>>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        var locations = await _uow.Repository<Location>().Query()
            .Include(l => l.Type)
            .Include(l => l.Parent)
            .Include(l => l.Children)
            .Where(l => l.ParentId == null)
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<LocationDto>>.SuccessResponse(_mapper.Map<List<LocationDto>>(locations));
    }

    public async Task<ApiResponse<LocationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var location = await _uow.Repository<Location>().Query()
            .Include(l => l.Type)
            .Include(l => l.Parent)
            .Include(l => l.Children)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (location == null)
            return ApiResponse<LocationDto>.FailResponse("Location not found.", "LOCATION_NOT_FOUND");

        return ApiResponse<LocationDto>.SuccessResponse(_mapper.Map<LocationDto>(location));
    }

    public async Task<ApiResponse<LocationDto>> CreateAsync(CreateLocationDto dto, CancellationToken cancellationToken = default)
    {
        var location = _mapper.Map<Location>(dto);

        if (dto.ParentId.HasValue)
        {
            var parent = await _uow.Repository<Location>().GetByIdAsync(dto.ParentId.Value, cancellationToken);
            if (parent == null)
                return ApiResponse<LocationDto>.FailResponse("Parent location not found.", "LOCATION_PARENT_NOT_FOUND");

            location.Level = parent.Level + 1;
        }
        else
        {
            location.Level = 0;
        }

        await _uow.Repository<Location>().AddAsync(location, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<LocationDto>.SuccessResponse(_mapper.Map<LocationDto>(location), "Location created successfully.");
    }

    public async Task<ApiResponse<LocationDto>> UpdateAsync(Guid id, UpdateLocationDto dto, CancellationToken cancellationToken = default)
    {
        var location = await _uow.Repository<Location>().GetByIdAsync(id, cancellationToken);
        if (location == null)
            return ApiResponse<LocationDto>.FailResponse("Location not found.", "LOCATION_NOT_FOUND");

        if (dto.Name != null) location.Name = dto.Name;
        if (dto.Code != null) location.Code = dto.Code;
        if (dto.TypeId.HasValue) location.TypeId = dto.TypeId;
        if (dto.IsActive.HasValue) location.IsActive = dto.IsActive.Value;

        if (dto.ParentId.HasValue && dto.ParentId != location.ParentId)
        {
            var parent = await _uow.Repository<Location>().GetByIdAsync(dto.ParentId.Value, cancellationToken);
            if (parent == null)
                return ApiResponse<LocationDto>.FailResponse("Parent location not found.", "LOCATION_PARENT_NOT_FOUND");

            location.ParentId = dto.ParentId;
            location.Level = parent.Level + 1;
        }

        _uow.Repository<Location>().Update(location);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<LocationDto>.SuccessResponse(_mapper.Map<LocationDto>(location), "Location updated successfully.");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var location = await _uow.Repository<Location>().GetByIdAsync(id, cancellationToken);
        if (location == null)
            return ApiResponse.FailResponse("Location not found.", "LOCATION_NOT_FOUND");

        _uow.Repository<Location>().SoftDelete(location);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse.SuccessResponse("Location deleted successfully.");
    }
}
