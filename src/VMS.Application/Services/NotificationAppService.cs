using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Notifications;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Enums;
using VMS.Core.Interfaces;

namespace VMS.Application.Services;

public class NotificationAppService : INotificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public NotificationAppService(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<PagedResult<NotificationDto>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _uow.Repository<Notification>().Query();

        if (_currentUser.UserId.HasValue)
            query = query.Where(n => n.RecipientUserId == _currentUser.UserId.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ApiResponse<PagedResult<NotificationDto>>.SuccessResponse(new PagedResult<NotificationDto>
        {
            Items = _mapper.Map<List<NotificationDto>>(notifications),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<ApiResponse> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _uow.Repository<Notification>().GetByIdAsync(id, cancellationToken);
        if (notification == null)
            return ApiResponse.FailResponse("Notification not found.", "NOTIFICATION_NOT_FOUND");

        notification.Status = NotificationStatus.Read;
        notification.ReadAt = DateTime.UtcNow;

        _uow.Repository<Notification>().Update(notification);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse.SuccessResponse("Notification marked as read.");
    }
}
