

# PaymentService Docker Compose Bilgileri

```yaml
paymentservice:
  image: paymentservice:latest
  ports:
    - "5241:5241"
  environment:
    - ConnectionStrings__DefaultConnection=Host=postgres;Database=PaymentDb;Username=postgres;Password=postgres
    - Jwt__Key=your-secret-key-here-must-be-at-least-32-characters
    - Jwt__Issuer=your-issuer
    - Jwt__Audience=your-audience
    - RabbitMQ__HostName=rabbitmq
    - RabbitMQ__UserName=guest
    - RabbitMQ__Password=guest
    - RabbitMQ__QueueName=payment_status_queue
  depends_on:
    - postgres
    - rabbitmq
```