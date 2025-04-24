

# InventoryService Docker Compose Bilgileri

```yaml
inventoryservice:
  image: inventoryservice:latest
  ports:
    - "5207:5207"
  environment:
    - ConnectionStrings__DefaultConnection=Host=postgres;Database=InventoryDb;Username=postgres;Password=postgres
    - Jwt__Key=your-secret-key-here-must-be-at-least-32-characters
    - Jwt__Issuer=your-issuer
    - Jwt__Audience=your-audience
    - RabbitMQ__HostName=rabbitmq
    - RabbitMQ__UserName=guest
    - RabbitMQ__Password=guest
    - RabbitMQ__QueueName=inventory_queue
  depends_on:
    - postgres
    - rabbitmq
```


