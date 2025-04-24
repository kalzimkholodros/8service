using System;

namespace InventoryService.Application.DTOs;

public class InventoryDTO
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Warehouse { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateInventoryDTO
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Warehouse { get; set; }
}

public class UpdateInventoryDTO
{
    public int Quantity { get; set; }
    public string? Warehouse { get; set; }
} 