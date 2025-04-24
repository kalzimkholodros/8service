using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NotificationService.Application.Services;
using NotificationService.Application.DTOs;

namespace NotificationService.Infrastructure.Services;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private readonly string _queueName;

    public RabbitMQConsumer(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<RabbitMQSettings> rabbitMQSettings,
        ILogger<RabbitMQConsumer> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _queueName = rabbitMQSettings.Value.QueueName;

        var factory = new ConnectionFactory
        {
            HostName = rabbitMQSettings.Value.HostName,
            UserName = rabbitMQSettings.Value.UserName,
            Password = rabbitMQSettings.Value.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var notificationDto = JsonSerializer.Deserialize<CreateNotificationDTO>(message);

                if (notificationDto != null)
                {
                    await notificationService.CreateNotificationAsync(notificationDto);
                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Notification created from message");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notification message");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: _queueName,
                            autoAck: false,
                            consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}

public class RabbitMQSettings
{
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
} 