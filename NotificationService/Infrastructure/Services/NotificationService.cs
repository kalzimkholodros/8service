using Microsoft.EntityFrameworkCore;
using NotificationService.Application.DTOs;
using NotificationService.Application.Services;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly NotificationDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(NotificationDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<NotificationDTO> CreateNotificationAsync(CreateNotificationDTO notificationDto)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = notificationDto.UserId,
            Type = notificationDto.Type,
            Message = notificationDto.Message,
            Status = "Queued",
            CreatedAt = DateTime.UtcNow
        };

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        // Simüle edilmiş bildirim gönderme
        // Gerçek uygulamada burada ilgili bildirim servisi (Email, SMS, Push) çağrılır
        await Task.Delay(1000);
        notification.Status = "Sent";
        await _context.SaveChangesAsync();

        _logger.LogInformation("Notification sent to user {UserId}", notificationDto.UserId);
        return MapToDTO(notification);
    }

    public async Task<List<NotificationDTO>> GetUserNotificationsAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(MapToDTO).ToList();
    }

    public async Task<NotificationDTO> SendTestNotificationAsync(TestNotificationDTO testDto)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(), // Test için rastgele bir kullanıcı ID'si
            Type = testDto.Type,
            Message = testDto.Message,
            Status = "Queued",
            CreatedAt = DateTime.UtcNow
        };

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        // Simüle edilmiş bildirim gönderme
        await Task.Delay(1000);
        notification.Status = "Sent";
        await _context.SaveChangesAsync();

        _logger.LogInformation("Test notification sent with type {Type}", testDto.Type);
        return MapToDTO(notification);
    }

    private static NotificationDTO MapToDTO(Notification notification)
    {
        return new NotificationDTO
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type,
            Message = notification.Message,
            Status = notification.Status,
            CreatedAt = notification.CreatedAt
        };
    }
} 