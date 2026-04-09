using Microsoft.Extensions.Logging;
using VMS.Application.Interfaces;
using VMS.Core.Enums;

namespace VMS.Infrastructure.Services.Notifications;

public class EmailNotificationChannel : INotificationChannel
{
    private readonly ILogger<EmailNotificationChannel> _logger;

    public EmailNotificationChannel(ILogger<EmailNotificationChannel> logger)
    {
        _logger = logger;
    }

    public NotificationChannel Channel => NotificationChannel.Email;

    public Task SendAsync(string recipient, string title, string message, CancellationToken cancellationToken = default)
    {
        // Mock implementation — replace with actual SMTP/SendGrid integration
        _logger.LogInformation("[EMAIL] To: {Recipient} | Subject: {Title} | Body: {Message}", recipient, title, message);
        return Task.CompletedTask;
    }
}

public class SmsNotificationChannel : INotificationChannel
{
    private readonly ILogger<SmsNotificationChannel> _logger;

    public SmsNotificationChannel(ILogger<SmsNotificationChannel> logger)
    {
        _logger = logger;
    }

    public NotificationChannel Channel => NotificationChannel.SMS;

    public Task SendAsync(string recipient, string title, string message, CancellationToken cancellationToken = default)
    {
        // Mock implementation — replace with actual SMS gateway (Twilio, etc.)
        _logger.LogInformation("[SMS] To: {Recipient} | Message: {Title} - {Message}", recipient, title, message);
        return Task.CompletedTask;
    }
}

public class WhatsAppNotificationChannel : INotificationChannel
{
    private readonly ILogger<WhatsAppNotificationChannel> _logger;

    public WhatsAppNotificationChannel(ILogger<WhatsAppNotificationChannel> logger)
    {
        _logger = logger;
    }

    public NotificationChannel Channel => NotificationChannel.WhatsApp;

    public Task SendAsync(string recipient, string title, string message, CancellationToken cancellationToken = default)
    {
        // Mock implementation — replace with actual WhatsApp Business API
        _logger.LogInformation("[WHATSAPP] To: {Recipient} | Message: {Title} - {Message}", recipient, title, message);
        return Task.CompletedTask;
    }
}

public class InAppNotificationChannel : INotificationChannel
{
    private readonly ILogger<InAppNotificationChannel> _logger;

    public InAppNotificationChannel(ILogger<InAppNotificationChannel> logger)
    {
        _logger = logger;
    }

    public NotificationChannel Channel => NotificationChannel.InApp;

    public Task SendAsync(string recipient, string title, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[IN-APP] To: {Recipient} | Title: {Title} | Message: {Message}", recipient, title, message);
        return Task.CompletedTask;
    }
}
