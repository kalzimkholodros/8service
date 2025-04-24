# ReviewService Docker Compose Bilgileri

```yaml
reviewservice:
  image: reviewservice:latest
  ports:
    - "5174:5174"
  environment:
    - ConnectionStrings__DefaultConnection=Host=postgres;Database=ReviewDb;Username=postgres;Password=postgres
    - IdentityServer__Authority=https://authservice:5206
    - RabbitMQ__HostName=rabbitmq
    - RabbitMQ__UserName=guest
    - RabbitMQ__Password=guest
    - RabbitMQ__QueueName=review_notification_queue
  depends_on:
    - postgres
    - authservice
    - rabbitmq
