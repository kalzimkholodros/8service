using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.DTOs;
using NotificationService.Application.Services;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<NotificationDTO>>> GetUserNotifications(Guid userId)
    {
        try
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("test")]
    public async Task<ActionResult<NotificationDTO>> SendTestNotification([FromBody] TestNotificationDTO testDto)
    {
        try
        {
            var notification = await _notificationService.SendTestNotificationAsync(testDto);
            return Ok(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending test notification");
            return BadRequest(ex.Message);
        }
    }
} 