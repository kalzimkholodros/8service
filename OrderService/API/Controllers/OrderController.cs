using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Services;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{userId}")]
    [Authorize]
    public async Task<ActionResult<List<OrderDTO>>> GetUserOrders(Guid userId)
    {
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet("detail/{id}")]
    [Authorize]
    public async Task<ActionResult<OrderDTO>> GetOrderDetail(Guid id)
    {
        var order = await _orderService.GetOrderDetailAsync(id);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    [HttpPut("status/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDTO statusDto)
    {
        await _orderService.UpdateOrderStatusAsync(id, statusDto.Status);
        return Ok();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<OrderDTO>>> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }
} 