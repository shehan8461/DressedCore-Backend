using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Service.Services;
using System.Security.Claims;

namespace Payment.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ITransactionService _transactionService;

    public PaymentsController(IPaymentService paymentService, ITransactionService transactionService)
    {
        _paymentService = paymentService;
        _transactionService = transactionService;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication" });
            }

            var result = await _paymentService.ProcessPaymentAsync(
                request.OrderId,
                userId,
                request.Amount,
                request.PaymentMethod
            );

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error processing payment: {ex.Message}" });
        }
    }

    [HttpGet("{paymentId}/status")]
    public async Task<IActionResult> GetPaymentStatus(int paymentId)
    {
        try
        {
            var status = await _paymentService.GetPaymentStatusAsync(paymentId);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = $"Payment not found: {ex.Message}" });
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserPayments()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication" });
            }

            var payments = await _paymentService.GetUserPaymentsAsync(userId);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error retrieving payments: {ex.Message}" });
        }
    }

    [HttpPost("{paymentId}/refund")]
    public async Task<IActionResult> RefundPayment(int paymentId, [FromBody] RefundRequest request)
    {
        try
        {
            var success = await _paymentService.RefundPaymentAsync(paymentId, request.Amount, request.Reason);
            if (success)
            {
                return Ok(new { message = "Refund processed successfully" });
            }
            return StatusCode(500, new { message = "Refund failed" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error processing refund: {ex.Message}" });
        }
    }

    [HttpGet("{paymentId}/transactions")]
    public async Task<IActionResult> GetPaymentTransactions(int paymentId)
    {
        try
        {
            var transactions = await _transactionService.GetPaymentTransactionsAsync(paymentId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error retrieving transactions: {ex.Message}" });
        }
    }

    [HttpPost("calculate-fee")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateFee([FromBody] CalculateFeeRequest request)
    {
        try
        {
            var fee = await _paymentService.CalculatePlatformFeeAsync(request.Amount);
            var total = request.Amount + fee;
            return Ok(new { platformFee = fee, totalAmount = total });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error calculating fee: {ex.Message}" });
        }
    }
}

public record ProcessPaymentRequest(
    int OrderId,
    decimal Amount,
    string PaymentMethod
);

public record RefundRequest(
    decimal Amount,
    string Reason
);

public record CalculateFeeRequest(
    decimal Amount
);
