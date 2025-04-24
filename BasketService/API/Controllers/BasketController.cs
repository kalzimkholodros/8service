using BasketService.Application.DTOs;
using BasketService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace BasketService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<BasketController> _logger;

    public BasketController(
        IBasketService basketService,
        IConnectionMultiplexer redis,
        ILogger<BasketController> logger)
    {
        _basketService = basketService;
        _redis = redis;
        _logger = logger;
    }

    [HttpGet("test-redis")]
    [AllowAnonymous]
    public async Task<IActionResult> TestRedis()
    {
        try
        {
            var db = _redis.GetDatabase();
            
            // Test yazma
            await db.StringSetAsync("test-key", "test-value");
            _logger.LogInformation("Redis write test successful");
            
            // Test okuma
            var value = await db.StringGetAsync("test-key");
            _logger.LogInformation("Redis read test successful. Value: {Value}", value);
            
            // Test silme
            await db.KeyDeleteAsync("test-key");
            _logger.LogInformation("Redis delete test successful");
            
            return Ok(new { 
                message = "Redis tests successful",
                readValue = value.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis test failed");
            return StatusCode(500, new { error = "Redis test failed", details = ex.Message });
        }
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<BasketDto>> GetBasket(string userId)
    {
        var basket = await _basketService.GetBasketAsync(userId);
        if (basket == null)
        {
            return NotFound();
        }
        return Ok(basket);
    }

    [HttpPost]
    public async Task<ActionResult<BasketDto>> AddItemToBasket([FromBody] AddItemToBasketDto item)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var basket = await _basketService.AddItemToBasketAsync(userId, item);
        return Ok(basket);
    }

    [HttpPut("{productId}")]
    public async Task<ActionResult<BasketDto>> UpdateBasketItem(Guid productId, [FromBody] UpdateBasketItemDto item)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var basket = await _basketService.UpdateBasketItemAsync(userId, productId, item);
        return Ok(basket);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteBasket()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        await _basketService.DeleteBasketAsync(userId);
        return NoContent();
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteBasketItem(Guid productId)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        await _basketService.DeleteBasketItemAsync(userId, productId);
        return NoContent();
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CheckoutBasket([FromBody] CheckoutBasketDto checkoutDto)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        checkoutDto.UserId = userId;
        await _basketService.CheckoutBasketAsync(checkoutDto);
        return Ok();
    }
} 