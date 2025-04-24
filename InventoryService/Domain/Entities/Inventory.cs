using System;

namespace InventoryService.Domain.Entities;

public class Inventory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Warehouse { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 