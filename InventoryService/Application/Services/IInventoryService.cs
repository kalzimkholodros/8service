using InventoryService.Application.DTOs;

namespace InventoryService.Application.Services;

public interface IInventoryService
{
    Task<InventoryDTO> GetInventoryByProductIdAsync(Guid productId);
    Task<InventoryDTO> CreateInventoryAsync(CreateInventoryDTO inventoryDto);
    Task<InventoryDTO> DecreaseInventoryAsync(Guid id, int quantity);
    Task<InventoryDTO> IncreaseInventoryAsync(Guid id, int quantity);
    Task<List<InventoryDTO>> GetAllInventoriesAsync();
} 