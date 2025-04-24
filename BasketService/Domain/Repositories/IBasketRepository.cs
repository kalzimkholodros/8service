using BasketService.Domain.Entities;

namespace BasketService.Domain.Repositories;

public interface IBasketRepository
{
    Task<Basket?> GetBasketByUserIdAsync(string userId);
    Task<Basket> CreateBasketAsync(Basket basket);
    Task<Basket> UpdateBasketAsync(Basket basket);
    Task DeleteBasketAsync(string userId);
    Task DeleteBasketItemAsync(string userId, Guid productId);
} 