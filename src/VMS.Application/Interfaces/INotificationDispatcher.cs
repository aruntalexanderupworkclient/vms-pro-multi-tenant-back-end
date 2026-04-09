using VMS.Application.DTOs.Notifications;
using VMS.Core.Enums;

namespace VMS.Application.Interfaces;

public interface INotificationDispatcher
{
    Task SendNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default);
}
