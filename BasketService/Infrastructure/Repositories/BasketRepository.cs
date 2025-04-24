using Microsoft.EntityFrameworkCore;
using BasketService.Domain.Entities;
using BasketService.Domain.Repositories;
using BasketService.Infrastructure.Data;

namespace BasketService.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly BasketDbContext _context;

    public BasketRepository(BasketDbContext context)
    {
        _context = context;
    }

    public async Task<Basket?> GetBasketByUserIdAsync(string userId)
    {
        return await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == userId);
    }

    public async Task<Basket> CreateBasketAsync(Basket basket)
    {
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync();
        return basket;
    }

    public async Task<Basket> UpdateBasketAsync(Basket basket)
    {
        basket.UpdatedAt = DateTime.UtcNow;
        _context.Baskets.Update(basket);
        await _context.SaveChangesAsync();
        return basket;
    }

    public async Task DeleteBasketAsync(string userId)
    {
        var basket = await GetBasketByUserIdAsync(userId);
        if (basket != null)
        {
            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteBasketItemAsync(string userId, Guid productId)
    {
        var basket = await GetBasketByUserIdAsync(userId);
        if (basket != null)
        {
            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _context.BasketItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
} 