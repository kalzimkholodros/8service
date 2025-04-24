using OrderService.Application.DTOs;

namespace OrderService.Application.Services;

public interface IOrderService
{
    Task<List<OrderDTO>> GetUserOrdersAsync(Guid userId);
    Task<OrderDTO?> GetOrderDetailAsync(Guid id);
    Task UpdateOrderStatusAsync(Guid id, string status);
    Task<List<OrderDTO>> GetAllOrdersAsync();
    Task CreateOrderAsync(CreateOrderDTO orderDto);
} 