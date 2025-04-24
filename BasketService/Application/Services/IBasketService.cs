using BasketService.Application.DTOs;

namespace BasketService.Application.Services;

public interface IBasketService
{
    Task<BasketDto?> GetBasketAsync(string userId);
    Task<BasketDto> AddItemToBasketAsync(string userId, AddItemToBasketDto item);
    Task<BasketDto> UpdateBasketItemAsync(string userId, Guid productId, UpdateBasketItemDto item);
    Task DeleteBasketAsync(string userId);
    Task DeleteBasketItemAsync(string userId, Guid productId);
    Task CheckoutBasketAsync(CheckoutBasketDto checkoutDto);
} 