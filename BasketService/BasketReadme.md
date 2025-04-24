

# BasketService Docker Compose Bilgileri

```yaml
basketservice:
  image: basketservice:latest
  ports:
    - "5256:5256"
  environment:
    - ConnectionStrings__DefaultConnection=Host=postgres;Database=BasketDb;Username=postgres;Password=postgres
    - ConnectionStrings__Redis=redis:6379
    - Jwt__Authority=https://authservice:5206
    - Jwt__Audience=basket-service
    - ProductService__BaseUrl=https://productservice:5142
    - RabbitMQ__HostName=rabbitmq
    - RabbitMQ__UserName=guest
    - RabbitMQ__Password=guest
  depends_on:
    - postgres
    - redis
    - rabbitmq
```