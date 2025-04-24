

# AuthService Docker Compose Bilgileri

```yaml
authservice:
  image: authservice:latest
  ports:
    - "5206:5206"
  environment:
    - ConnectionStrings__DefaultConnection=Host=postgres;Database=AuthDb;Username=postgres;Password=postgres
    - Jwt__Key=super-secret-key-from-user-service-must-be-very-long-and-secure-123!@#
    - Jwt__Issuer=AuthService
    - Jwt__Audience=AuthServiceClients
    - Jwt__ExpiryInHours=1
  depends_on:
    - postgres
```
