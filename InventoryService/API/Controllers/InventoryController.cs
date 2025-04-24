using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryService.Application.DTOs;
using InventoryService.Application.Services;

namespace InventoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<InventoryDTO>> GetInventoryByProductId(Guid productId)
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
            return Ok(inventory);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory for product {ProductId}", productId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InventoryDTO>> CreateInventory([FromBody] CreateInventoryDTO inventoryDto)
    {
        try
        {
            var inventory = await _inventoryService.CreateInventoryAsync(inventoryDto);
            return CreatedAtAction(nameof(GetInventoryByProductId), new { productId = inventory.ProductId }, inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inventory");
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("decrease/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InventoryDTO>> DecreaseInventory(Guid id, [FromBody] int quantity)
    {
        try
        {
            var inventory = await _inventoryService.DecreaseInventoryAsync(id, quantity);
            return Ok(inventory);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decreasing inventory {InventoryId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("increase/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InventoryDTO>> IncreaseInventory(Guid id, [FromBody] int quantity)
    {
        try
        {
            var inventory = await _inventoryService.IncreaseInventoryAsync(id, quantity);
            return Ok(inventory);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error increasing inventory {InventoryId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<InventoryDTO>>> GetAllInventories()
    {
        var inventories = await _inventoryService.GetAllInventoriesAsync();
        return Ok(inventories);
    }
} 