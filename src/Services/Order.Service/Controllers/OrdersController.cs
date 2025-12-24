using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Service.Services;
using System.Security.Claims;

namespace Order.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication" });
            }

            var result = await _orderService.CreateOrderAsync(
                request.DesignId,
                request.DesignerId,
                request.SupplierId,
                request.QuoteId,
                request.Amount
            );

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error creating order: {ex.Message}" });
        }
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }
            return Ok(order);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error retrieving order: {ex.Message}" });
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserOrders()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication" });
            }

            if (string.IsNullOrEmpty(userTypeClaim))
            {
                return Unauthorized(new { message = "User type not found" });
            }

            var orders = await _orderService.GetUserOrdersAsync(userId, userTypeClaim);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error retrieving orders: {ex.Message}" });
        }
    }

    [HttpPut("{orderId}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var success = await _orderService.UpdateOrderStatusAsync(orderId, request.Status);
            if (success)
            {
                return Ok(new { message = "Order status updated successfully" });
            }
            return StatusCode(500, new { message = "Failed to update order status" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error updating order status: {ex.Message}" });
        }
    }

    [HttpPut("{orderId}/payment-status")]
    public async Task<IActionResult> UpdatePaymentStatus(int orderId, [FromBody] UpdatePaymentStatusRequest request)
    {
        try
        {
            var success = await _orderService.UpdatePaymentStatusAsync(orderId, request.PaymentStatus);
            if (success)
            {
                return Ok(new { message = "Payment status updated successfully" });
            }
            return StatusCode(500, new { message = "Failed to update payment status" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error updating payment status: {ex.Message}" });
        }
    }
}

public record CreateOrderRequest(int DesignId, int DesignerId, int SupplierId, int QuoteId, decimal Amount);
public record UpdateStatusRequest(string Status);
public record UpdatePaymentStatusRequest(string PaymentStatus);
