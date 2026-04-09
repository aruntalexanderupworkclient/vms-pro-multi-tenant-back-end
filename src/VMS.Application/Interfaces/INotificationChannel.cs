using VMS.Core.Enums;

namespace VMS.Application.Interfaces;

public interface INotificationChannel
{
    NotificationChannel Channel { get; }
    Task SendAsync(string recipient, string title, string message, CancellationToken cancellationToken = default);
}
