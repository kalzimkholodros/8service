using Microsoft.EntityFrameworkCore;
using InventoryService.Application.DTOs;
using InventoryService.Application.Services;
using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Data;

namespace InventoryService.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(InventoryDbContext context, ILogger<InventoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<InventoryDTO> GetInventoryByProductIdAsync(Guid productId)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (inventory == null)
            throw new KeyNotFoundException($"Inventory for product {productId} not found");

        return MapToDTO(inventory);
    }

    public async Task<InventoryDTO> CreateInventoryAsync(CreateInventoryDTO inventoryDto)
    {
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = inventoryDto.ProductId,
            Quantity = inventoryDto.Quantity,
            Warehouse = inventoryDto.Warehouse,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Inventories.AddAsync(inventory);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New inventory created for product {ProductId}", inventoryDto.ProductId);
        return MapToDTO(inventory);
    }

    public async Task<InventoryDTO> DecreaseInventoryAsync(Guid id, int quantity)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null)
            throw new KeyNotFoundException($"Inventory with ID {id} not found");

        if (inventory.Quantity < quantity)
            throw new InvalidOperationException("Insufficient inventory");

        inventory.Quantity -= quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Inventory {InventoryId} decreased by {Quantity}", id, quantity);
        return MapToDTO(inventory);
    }

    public async Task<InventoryDTO> IncreaseInventoryAsync(Guid id, int quantity)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null)
            throw new KeyNotFoundException($"Inventory with ID {id} not found");

        inventory.Quantity += quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Inventory {InventoryId} increased by {Quantity}", id, quantity);
        return MapToDTO(inventory);
    }

    public async Task<List<InventoryDTO>> GetAllInventoriesAsync()
    {
        var inventories = await _context.Inventories
            .OrderByDescending(i => i.UpdatedAt)
            .ToListAsync();

        return inventories.Select(MapToDTO).ToList();
    }

    private static InventoryDTO MapToDTO(Inventory inventory)
    {
        return new InventoryDTO
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            Quantity = inventory.Quantity,
            Warehouse = inventory.Warehouse,
            UpdatedAt = inventory.UpdatedAt
        };
    }
} 