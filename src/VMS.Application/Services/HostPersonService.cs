using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Hosts;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Interfaces;

namespace VMS.Application.Services;

public class HostPersonService : IHostPersonService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public HostPersonService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<HostPersonDto>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _uow.Repository<HostPerson>().Query()
            .Include(h => h.Location);

        var totalCount = await query.CountAsync(cancellationToken);
        var hosts = await query
            .OrderByDescending(h => h.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ApiResponse<PagedResult<HostPersonDto>>.SuccessResponse(new PagedResult<HostPersonDto>
        {
            Items = _mapper.Map<List<HostPersonDto>>(hosts),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<ApiResponse<HostPersonDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var host = await _uow.Repository<HostPerson>().Query()
            .Include(h => h.Location)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

        if (host == null)
            return ApiResponse<HostPersonDto>.FailResponse("Host not found.", "HOST_NOT_FOUND");

        return ApiResponse<HostPersonDto>.SuccessResponse(_mapper.Map<HostPersonDto>(host));
    }

    public async Task<ApiResponse<HostPersonDto>> CreateAsync(CreateHostPersonDto dto, CancellationToken cancellationToken = default)
    {
        var host = _mapper.Map<HostPerson>(dto);

        await _uow.Repository<HostPerson>().AddAsync(host, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<HostPersonDto>.SuccessResponse(_mapper.Map<HostPersonDto>(host), "Host created successfully.");
    }

    public async Task<ApiResponse<HostPersonDto>> UpdateAsync(Guid id, UpdateHostPersonDto dto, CancellationToken cancellationToken = default)
    {
        var host = await _uow.Repository<HostPerson>().GetByIdAsync(id, cancellationToken);

        if (host == null)
            return ApiResponse<HostPersonDto>.FailResponse("Host not found.", "HOST_NOT_FOUND");

        if (dto.FullName != null) host.FullName = dto.FullName;
        if (dto.Email != null) host.Email = dto.Email;
        if (dto.Phone != null) host.Phone = dto.Phone;
        if (dto.Department != null) host.Department = dto.Department;
        if (dto.Designation != null) host.Designation = dto.Designation;
        if (dto.LocationId.HasValue) host.LocationId = dto.LocationId;
        if (dto.UserId.HasValue) host.UserId = dto.UserId;
        if (dto.IsActive.HasValue) host.IsActive = dto.IsActive.Value;

        _uow.Repository<HostPerson>().Update(host);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<HostPersonDto>.SuccessResponse(_mapper.Map<HostPersonDto>(host), "Host updated successfully.");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var host = await _uow.Repository<HostPerson>().GetByIdAsync(id, cancellationToken);
        if (host == null)
            return ApiResponse.FailResponse("Host not found.", "HOST_NOT_FOUND");

        var hasActiveVisits = await _uow.Repository<Visit>().AnyAsync(
            v => v.HostId == id && (v.Status == VMS.Core.Enums.VisitStatus.Scheduled || v.Status == VMS.Core.Enums.VisitStatus.CheckedIn),
            cancellationToken);

        if (hasActiveVisits)
            return ApiResponse.FailResponse("Cannot delete host with active visits.", "HOST_HAS_ACTIVE_VISITS");

        _uow.Repository<HostPerson>().SoftDelete(host);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse.SuccessResponse("Host deleted successfully.");
    }
}
