using System;

namespace NotificationService.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty; // Email, SMS, Push, InApp
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Queued"; // Sent, Failed, Queued
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 