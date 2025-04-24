# OrderService Docker Compose Bilgileri

```yaml
orderservice:
  image: orderservice:latest
  ports:
    - "5163:5163"
  environment:
    - ConnectionStrings__DefaultConnection=Host=postgres;Database=OrderDb;Username=postgres;Password=postgres
    - Jwt__Key=your-secret-key-here-must-be-at-least-32-characters
    - Jwt__Issuer=your-issuer
    - Jwt__Audience=your-audience
    - RabbitMQ__HostName=rabbitmq
    - RabbitMQ__UserName=guest
    - RabbitMQ__Password=guest
    - RabbitMQ__QueueName=checkout_queue
  depends_on:
    - postgres
    - rabbitmq
