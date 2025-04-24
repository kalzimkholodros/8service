namespace BasketService.Application.DTOs;

public class BasketDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<BasketItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BasketItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class AddItemToBasketDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateBasketItemDto
{
    public int Quantity { get; set; }
}

public class CheckoutBasketDto
{
    public string UserId { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
} 