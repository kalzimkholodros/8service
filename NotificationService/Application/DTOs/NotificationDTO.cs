using System;

namespace NotificationService.Application.DTOs;

public class NotificationDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateNotificationDTO
{
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class TestNotificationDTO
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
} 