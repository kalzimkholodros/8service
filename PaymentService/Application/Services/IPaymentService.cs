using PaymentService.Application.DTOs;

namespace PaymentService.Application.Services;

public interface IPaymentService
{
    Task<PaymentDTO> CreatePaymentAsync(CreatePaymentDTO paymentDto);
    Task<PaymentDTO> ProcessPaymentAsync(Guid id, ProcessPaymentDTO processDto);
    Task<PaymentDTO?> GetPaymentAsync(Guid id);
    Task<List<PaymentDTO>> GetUserPaymentsAsync(Guid userId);
    Task<List<PaymentDTO>> GetAllPaymentsAsync();
    Task RefundPaymentAsync(Guid id, string reason);
} 