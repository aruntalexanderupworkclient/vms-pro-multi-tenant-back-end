using Microsoft.Extensions.Logging;
using VMS.Application.DTOs.Notifications;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Enums;
using VMS.Core.Interfaces;

namespace VMS.Infrastructure.Services.Notifications;

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly IEnumerable<INotificationChannel> _channels;
    private readonly IUnitOfWork _uow;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        IEnumerable<INotificationChannel> channels,
        IUnitOfWork uow,
        ITenantProvider tenantProvider,
        ILogger<NotificationDispatcher> logger)
    {
        _channels = channels;
        _uow = uow;
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    public async Task SendNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        // Persist notification record
        var notification = new Notification
        {
            Title = dto.Title,
            Message = dto.Message,
            Channel = dto.Channel,
            RecipientUserId = dto.RecipientUserId,
            RecipientEmail = dto.RecipientEmail,
            RecipientPhone = dto.RecipientPhone,
            ReferenceId = dto.ReferenceId,
            ReferenceType = dto.ReferenceType,
            Status = NotificationStatus.Pending,
            TenantId = _tenantProvider.GetTenantId()
        };

        await _uow.Repository<Notification>().AddAsync(notification, cancellationToken);

        // Dispatch through the appropriate channel
        var channel = _channels.FirstOrDefault(c => c.Channel == dto.Channel);
        if (channel != null)
        {
            var recipient = dto.Channel switch
            {
                NotificationChannel.Email => dto.RecipientEmail ?? "unknown",
                NotificationChannel.SMS => dto.RecipientPhone ?? "unknown",
                NotificationChannel.WhatsApp => dto.RecipientPhone ?? "unknown",
                NotificationChannel.InApp => dto.RecipientUserId?.ToString() ?? "unknown",
                _ => "unknown"
            };

            try
            {
                await channel.SendAsync(recipient, dto.Title, dto.Message, cancellationToken);
                notification.Status = NotificationStatus.Sent;
                notification.SentAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification via {Channel}", dto.Channel);
                notification.Status = NotificationStatus.Failed;
            }

            _uow.Repository<Notification>().Update(notification);
        }

        await _uow.SaveChangesAsync(cancellationToken);
    }
}
