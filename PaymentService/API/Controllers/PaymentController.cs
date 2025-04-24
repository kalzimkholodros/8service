using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.DTOs;
using PaymentService.Application.Services;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PaymentDTO>> CreatePayment([FromBody] CreatePaymentDTO paymentDto)
    {
        try
        {
            var payment = await _paymentService.CreatePaymentAsync(paymentDto);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("process/{id}")]
    [Authorize]
    public async Task<ActionResult<PaymentDTO>> ProcessPayment(Guid id, [FromBody] ProcessPaymentDTO processDto)
    {
        try
        {
            var payment = await _paymentService.ProcessPaymentAsync(id, processDto);
            return Ok(payment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment {PaymentId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<PaymentDTO>> GetPayment(Guid id)
    {
        var payment = await _paymentService.GetPaymentAsync(id);
        if (payment == null)
            return NotFound();
        return Ok(payment);
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<List<PaymentDTO>>> GetUserPayments(Guid userId)
    {
        var payments = await _paymentService.GetUserPaymentsAsync(userId);
        return Ok(payments);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PaymentDTO>>> GetAllPayments()
    {
        var payments = await _paymentService.GetAllPaymentsAsync();
        return Ok(payments);
    }

    [HttpPut("refund/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RefundPayment(Guid id, [FromBody] RefundPaymentDTO refundDto)
    {
        try
        {
            await _paymentService.RefundPaymentAsync(id, refundDto.Reason);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment {PaymentId}", id);
            return BadRequest(ex.Message);
        }
    }
} 