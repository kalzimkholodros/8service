# API Gateway Docker Compose Bilgileri

```yaml
services:
  apigateway:
    image: apigateway:latest
    ports:
      - "5182:5182"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:5182
    depends_on:
      - authservice
      - productservice
      - basketservice
      - orderservice
      - paymentservice
      - reviewservice
      - notificationservice
      - inventoryservice
    networks:
      - ecommerce-network
```

## Port Bilgileri
- API Gateway: 5182

## Bağımlılıklar
- Auth Service (5206)
- Product Service (5142)
- Basket Service (5256)
- Order Service (5163)
- Payment Service (5241)
- Review Service (5174)
- Notification Service (5072)
- Inventory Service (5294)

## Environment Variables
- ASPNETCORE_ENVIRONMENT: Development
- ASPNETCORE_URLS: http://+:5182

## Network
- E-commerce ağına bağlı (ecommerce-network)
