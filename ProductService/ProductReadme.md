# ProductService Docker Compose Bilgileri

```yaml
productservice:
  image: productservice:latest
  ports:
    - "5205:5205"
  environment:
    - ConnectionStrings__DefaultConnection=Host=postgres;Database=ProductDb;Username=postgres;Password=postgres
    - Jwt__Authority=https://authservice:5206
    - Jwt__Audience=product-service
  depends_on:
    - postgres
    - authservice
