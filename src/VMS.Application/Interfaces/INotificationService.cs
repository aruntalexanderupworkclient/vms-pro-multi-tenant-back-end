using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Notifications;

namespace VMS.Application.Interfaces;

public interface INotificationService
{
    Task<ApiResponse<PagedResult<NotificationDto>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResponse> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
}
