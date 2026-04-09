using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Notifications;
using VMS.Application.DTOs.Visitors;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Enums;
using VMS.Core.Interfaces;
using VMS.Shared.Helpers;

namespace VMS.Application.Services;

public class VisitService : IVisitService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly INotificationDispatcher _notificationDispatcher;

    public VisitService(IUnitOfWork uow, IMapper mapper, INotificationDispatcher notificationDispatcher)
    {
        _uow = uow;
        _mapper = mapper;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task<ApiResponse<PagedResult<VisitDto>>> GetAllAsync(Guid? visitorId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _uow.Repository<Visit>().Query()
            .Include(v => v.Visitor)
            .Include(v => v.Host)
            .Include(v => v.Location)
            .AsQueryable();

        if (visitorId.HasValue)
            query = query.Where(v => v.VisitorId == visitorId.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var visits = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ApiResponse<PagedResult<VisitDto>>.SuccessResponse(new PagedResult<VisitDto>
        {
            Items = _mapper.Map<List<VisitDto>>(visits),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<ApiResponse<VisitDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var visit = await _uow.Repository<Visit>().Query()
            .Include(v => v.Visitor)
            .Include(v => v.Host)
            .Include(v => v.Location)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (visit == null)
            return ApiResponse<VisitDto>.FailResponse("Visit not found.", "VISIT_NOT_FOUND");

        return ApiResponse<VisitDto>.SuccessResponse(_mapper.Map<VisitDto>(visit));
    }

    public async Task<ApiResponse<VisitDto>> CreateAsync(CreateVisitDto dto, CancellationToken cancellationToken = default)
    {
        var visit = _mapper.Map<Visit>(dto);
        visit.QRCode = QrCodeGenerator.GenerateQrCodeString(visit.Id);
        visit.Status = VisitStatus.Scheduled;

        await _uow.Repository<Visit>().AddAsync(visit, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var created = await _uow.Repository<Visit>().Query()
            .Include(v => v.Visitor)
            .Include(v => v.Host)
            .Include(v => v.Location)
            .FirstOrDefaultAsync(v => v.Id == visit.Id, cancellationToken);

        return ApiResponse<VisitDto>.SuccessResponse(_mapper.Map<VisitDto>(created), "Visit created successfully.");
    }

    public async Task<ApiResponse<VisitDto>> CheckInAsync(Guid visitId, CheckInDto dto, CancellationToken cancellationToken = default)
    {
        var visit = await _uow.Repository<Visit>().Query()
            .Include(v => v.Visitor)
            .Include(v => v.Host)
            .Include(v => v.Location)
            .FirstOrDefaultAsync(v => v.Id == visitId, cancellationToken);

        if (visit == null)
            return ApiResponse<VisitDto>.FailResponse("Visit not found.", "VISIT_NOT_FOUND");

        if (visit.Status != VisitStatus.Scheduled)
            return ApiResponse<VisitDto>.FailResponse("Visit cannot be checked in.", "VISIT_INVALID_STATUS");

        visit.Status = VisitStatus.CheckedIn;
        visit.CheckInTime = DateTime.UtcNow;
        visit.AccessCardNumber = dto.AccessCardNumber;
        visit.AccessCardIssuedAt = string.IsNullOrEmpty(dto.AccessCardNumber) ? null : DateTime.UtcNow;

        _uow.Repository<Visit>().Update(visit);
        await _uow.SaveChangesAsync(cancellationToken);

        if (visit.Host != null)
        {
            await _notificationDispatcher.SendNotificationAsync(new CreateNotificationDto
            {
                Title = "Visitor Checked In",
                Message = $"Visitor {visit.Visitor?.FullName} has checked in.",
                Channel = NotificationChannel.InApp,
                RecipientUserId = visit.Host.UserId,
                ReferenceId = visit.Id,
                ReferenceType = "Visit"
            }, cancellationToken);
        }

        return ApiResponse<VisitDto>.SuccessResponse(_mapper.Map<VisitDto>(visit), "Visitor checked in successfully.");
    }

    public async Task<ApiResponse<VisitDto>> CheckOutAsync(Guid visitId, CheckOutDto dto, CancellationToken cancellationToken = default)
    {
        var visit = await _uow.Repository<Visit>().Query()
            .Include(v => v.Visitor)
            .Include(v => v.Host)
            .Include(v => v.Location)
            .FirstOrDefaultAsync(v => v.Id == visitId, cancellationToken);

        if (visit == null)
            return ApiResponse<VisitDto>.FailResponse("Visit not found.", "VISIT_NOT_FOUND");

        if (visit.Status != VisitStatus.CheckedIn)
            return ApiResponse<VisitDto>.FailResponse("Visit cannot be checked out.", "VISIT_INVALID_STATUS");

        visit.Status = VisitStatus.CheckedOut;
        visit.CheckOutTime = DateTime.UtcNow;
        visit.AccessCardReturnedAt = DateTime.UtcNow;
        visit.Remarks = dto.Remarks ?? visit.Remarks;

        _uow.Repository<Visit>().Update(visit);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<VisitDto>.SuccessResponse(_mapper.Map<VisitDto>(visit), "Visitor checked out successfully.");
    }
}
