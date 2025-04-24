using BasketService.Application.DTOs;
using BasketService.Application.Services;
using BasketService.Domain.Entities;
using BasketService.Domain.Repositories;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace BasketService.Application.Services;

public class BasketService : IBasketService
{
    private readonly IBasketRepository _basketRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConnectionMultiplexer _redis;
    private readonly IModel _rabbitMqChannel;
    private readonly ILogger<BasketService> _logger;
    private const string RedisKeyPrefix = "basket:";

    public BasketService(
        IBasketRepository basketRepository,
        IHttpClientFactory httpClientFactory,
        IConnectionMultiplexer redis,
        IModel rabbitMqChannel,
        ILogger<BasketService> logger)
    {
        _basketRepository = basketRepository;
        _httpClientFactory = httpClientFactory;
        _redis = redis;
        _rabbitMqChannel = rabbitMqChannel;
        _logger = logger;
        
        // Test Redis connection
        try
        {
            var db = _redis.GetDatabase();
            db.StringSet("test", "test");
            var value = db.StringGet("test");
            _logger.LogInformation("Redis connection test successful. Value: {Value}", value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis connection test failed");
        }
    }

    public async Task<BasketDto?> GetBasketAsync(string userId)
    {
        // Try to get from Redis first
        var redisDb = _redis.GetDatabase();
        var redisKey = $"{RedisKeyPrefix}{userId}";
        var redisBasket = await redisDb.StringGetAsync(redisKey);

        if (redisBasket.HasValue)
        {
            return JsonSerializer.Deserialize<BasketDto>(redisBasket!);
        }

        // If not in Redis, get from database
        var basket = await _basketRepository.GetBasketByUserIdAsync(userId);
        if (basket == null) return null;

        var basketDto = MapToDto(basket);
        
        // Cache in Redis
        await redisDb.StringSetAsync(redisKey, JsonSerializer.Serialize(basketDto), TimeSpan.FromMinutes(30));

        return basketDto;
    }

    public async Task<BasketDto> AddItemToBasketAsync(string userId, AddItemToBasketDto item)
    {
        // Get product details from ProductService
        var httpClient = _httpClientFactory.CreateClient("ProductService");
        var productResponse = await httpClient.GetAsync($"api/products/{item.ProductId}");
        productResponse.EnsureSuccessStatusCode();
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

        if (product == null)
        {
            throw new Exception("Product not found");
        }

        var basket = await _basketRepository.GetBasketByUserIdAsync(userId) ?? new Basket { UserId = userId };
        
        var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            basket.Items.Add(new BasketItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = item.Quantity
            });
        }

        basket = await _basketRepository.UpdateBasketAsync(basket);

        // Update Redis cache
        var redisDb = _redis.GetDatabase();
        var redisKey = $"{RedisKeyPrefix}{userId}";
        await redisDb.StringSetAsync(redisKey, JsonSerializer.Serialize(MapToDto(basket)), TimeSpan.FromMinutes(30));

        return MapToDto(basket);
    }

    public async Task<BasketDto> UpdateBasketItemAsync(string userId, Guid productId, UpdateBasketItemDto item)
    {
        var basket = await _basketRepository.GetBasketByUserIdAsync(userId);
        if (basket == null)
        {
            throw new Exception("Basket not found");
        }

        var basketItem = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (basketItem == null)
        {
            throw new Exception("Item not found in basket");
        }

        basketItem.Quantity = item.Quantity;
        basket = await _basketRepository.UpdateBasketAsync(basket);

        // Update Redis cache
        var redisDb = _redis.GetDatabase();
        var redisKey = $"{RedisKeyPrefix}{userId}";
        await redisDb.StringSetAsync(redisKey, JsonSerializer.Serialize(MapToDto(basket)), TimeSpan.FromMinutes(30));

        return MapToDto(basket);
    }

    public async Task DeleteBasketAsync(string userId)
    {
        await _basketRepository.DeleteBasketAsync(userId);

        // Remove from Redis cache
        var redisDb = _redis.GetDatabase();
        var redisKey = $"{RedisKeyPrefix}{userId}";
        await redisDb.KeyDeleteAsync(redisKey);
    }

    public async Task DeleteBasketItemAsync(string userId, Guid productId)
    {
        await _basketRepository.DeleteBasketItemAsync(userId, productId);

        // Update Redis cache
        var basket = await _basketRepository.GetBasketByUserIdAsync(userId);
        if (basket != null)
        {
            var redisDb = _redis.GetDatabase();
            var redisKey = $"{RedisKeyPrefix}{userId}";
            await redisDb.StringSetAsync(redisKey, JsonSerializer.Serialize(MapToDto(basket)), TimeSpan.FromMinutes(30));
        }
    }

    public async Task CheckoutBasketAsync(CheckoutBasketDto checkoutDto)
    {
        var basket = await _basketRepository.GetBasketByUserIdAsync(checkoutDto.UserId);
        if (basket == null || !basket.Items.Any())
        {
            throw new Exception("Basket is empty");
        }

        // Create order event
        var orderEvent = new OrderCreatedEvent
        {
            UserId = checkoutDto.UserId,
            Items = basket.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList(),
            TotalPrice = basket.TotalPrice,
            ShippingAddress = checkoutDto.ShippingAddress,
            PaymentMethod = checkoutDto.PaymentMethod
        };

        // Publish event to RabbitMQ
        var message = JsonSerializer.Serialize(orderEvent);
        var body = Encoding.UTF8.GetBytes(message);
        _rabbitMqChannel.BasicPublish(
            exchange: "order_exchange",
            routingKey: "order.created",
            basicProperties: null,
            body: body
        );

        // Clear the basket
        await DeleteBasketAsync(checkoutDto.UserId);
    }

    private BasketDto MapToDto(Basket basket)
    {
        return new BasketDto
        {
            Id = basket.Id,
            UserId = basket.UserId,
            Items = basket.Items.Select(i => new BasketItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList(),
            TotalPrice = basket.TotalPrice,
            CreatedAt = basket.CreatedAt,
            UpdatedAt = basket.UpdatedAt
        };
    }
}

// DTOs for external services
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class OrderCreatedEvent
{
    public string UserId { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
} 