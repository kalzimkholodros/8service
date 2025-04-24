using Microsoft.EntityFrameworkCore;
using PaymentService.Application.DTOs;
using PaymentService.Application.Services;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Data;

namespace PaymentService.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly PaymentDbContext _context;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(PaymentDbContext context, ILogger<PaymentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaymentDTO> CreatePaymentAsync(CreatePaymentDTO paymentDto)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = paymentDto.OrderId,
            UserId = paymentDto.UserId,
            Amount = paymentDto.Amount,
            Method = paymentDto.Method,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New payment created with ID {PaymentId}", payment.Id);
        return MapToDTO(payment);
    }

    public async Task<PaymentDTO> ProcessPaymentAsync(Guid id, ProcessPaymentDTO processDto)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
            throw new KeyNotFoundException($"Payment with ID {id} not found");

        // Simüle edilmiş ödeme işlemi
        // Gerçek uygulamada burada bir ödeme sağlayıcısı (gateway) ile iletişim kurulur
        await Task.Delay(1000); // Simüle edilmiş işlem süresi

        payment.Status = "Paid";
        payment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Payment {PaymentId} processed successfully", id);
        return MapToDTO(payment);
    }

    public async Task<PaymentDTO?> GetPaymentAsync(Guid id)
    {
        var payment = await _context.Payments.FindAsync(id);
        return payment != null ? MapToDTO(payment) : null;
    }

    public async Task<List<PaymentDTO>> GetUserPaymentsAsync(Guid userId)
    {
        var payments = await _context.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return payments.Select(MapToDTO).ToList();
    }

    public async Task<List<PaymentDTO>> GetAllPaymentsAsync()
    {
        var payments = await _context.Payments
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return payments.Select(MapToDTO).ToList();
    }

    public async Task RefundPaymentAsync(Guid id, string reason)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
            throw new KeyNotFoundException($"Payment with ID {id} not found");

        if (payment.Status != "Paid")
            throw new InvalidOperationException("Only paid payments can be refunded");

        // Simüle edilmiş iade işlemi
        // Gerçek uygulamada burada bir ödeme sağlayıcısı (gateway) ile iletişim kurulur
        await Task.Delay(1000); // Simüle edilmiş işlem süresi

        payment.Status = "Refunded";
        payment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Payment {PaymentId} refunded. Reason: {Reason}", id, reason);
    }

    private static PaymentDTO MapToDTO(Payment payment)
    {
        return new PaymentDTO
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            UserId = payment.UserId,
            Amount = payment.Amount,
            Status = payment.Status,
            Method = payment.Method,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
        };
    }
} 