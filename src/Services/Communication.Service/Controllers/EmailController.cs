using Communication.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Communication.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        try
        {
            var success = await _emailService.SendEmailAsync(
                request.ToEmail,
                request.ToName,
                request.Subject,
                request.Body
            );

            if (success)
            {
                return Ok(new { message = "Email sent successfully" });
            }
            return StatusCode(500, new { message = "Failed to send email" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error sending email: {ex.Message}" });
        }
    }

    [HttpPost("design-notification")]
    public async Task<IActionResult> SendDesignNotification([FromBody] DesignNotificationRequest request)
    {
        try
        {
            var success = await _emailService.SendDesignNotificationAsync(
                request.SupplierEmail,
                request.SupplierName,
                request.DesignTitle,
                request.DesignId
            );

            if (success)
            {
                return Ok(new { message = "Design notification sent successfully" });
            }
            return StatusCode(500, new { message = "Failed to send design notification" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error sending design notification: {ex.Message}" });
        }
    }

    [HttpPost("quote-notification")]
    public async Task<IActionResult> SendQuoteNotification([FromBody] QuoteNotificationRequest request)
    {
        try
        {
            var success = await _emailService.SendQuoteNotificationAsync(
                request.DesignerEmail,
                request.DesignerName,
                request.DesignTitle,
                request.QuoteId
            );

            if (success)
            {
                return Ok(new { message = "Quote notification sent successfully" });
            }
            return StatusCode(500, new { message = "Failed to send quote notification" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error sending quote notification: {ex.Message}" });
        }
    }

    [HttpPost("order-confirmation")]
    public async Task<IActionResult> SendOrderConfirmation([FromBody] OrderConfirmationRequest request)
    {
        try
        {
            var success = await _emailService.SendOrderConfirmationAsync(
                request.Email,
                request.Name,
                request.OrderNumber,
                request.Amount
            );

            if (success)
            {
                return Ok(new { message = "Order confirmation sent successfully" });
            }
            return StatusCode(500, new { message = "Failed to send order confirmation" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error sending order confirmation: {ex.Message}" });
        }
    }
}

public record EmailRequest(
    string ToEmail,
    string ToName,
    string Subject,
    string Body
);

public record DesignNotificationRequest(
    string SupplierEmail,
    string SupplierName,
    string DesignTitle,
    int DesignId
);

public record QuoteNotificationRequest(
    string DesignerEmail,
    string DesignerName,
    string DesignTitle,
    int QuoteId
);

public record OrderConfirmationRequest(
    string Email,
    string Name,
    string OrderNumber,
    decimal Amount
);
