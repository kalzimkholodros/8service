# NotificationService Docker Compose Bilgileri

```yaml
notificationservice:
  image: notificationservice:latest
  ports:
    - "5072:5072"
  environment:
    - ConnectionStrings__DefaultConnection=Host=postgres;Database=NotificationDb;Username=postgres;Password=postgres
    - RabbitMQ__HostName=rabbitmq
    - RabbitMQ__UserName=guest
    - RabbitMQ__Password=guest
    - RabbitMQ__QueueName=notification_queue
  depends_on:
    - postgres
    - rabbitmq
