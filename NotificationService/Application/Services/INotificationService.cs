using NotificationService.Application.DTOs;

namespace NotificationService.Application.Services;

public interface INotificationService
{
    Task<NotificationDTO> CreateNotificationAsync(CreateNotificationDTO notificationDto);
    Task<List<NotificationDTO>> GetUserNotificationsAsync(Guid userId);
    Task<NotificationDTO> SendTestNotificationAsync(TestNotificationDTO testDto);
} 