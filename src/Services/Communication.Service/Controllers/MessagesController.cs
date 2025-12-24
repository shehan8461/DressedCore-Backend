using Communication.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Communication.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var senderIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderIdClaim) || !int.TryParse(senderIdClaim, out var senderId))
            {
                return Unauthorized(new { message = "Invalid user authentication" });
            }

            var messageId = await _messageService.SendMessage(
                senderId,
                request.ReceiverId,
                request.Content,
                request.DesignId,
                request.QuoteId
            );

            return Ok(new { messageId, message = "Message sent successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error sending message: {ex.Message}" });
        }
    }

    [HttpGet("conversation/{userId}")]
    public async Task<IActionResult> GetConversation(int userId, [FromQuery] int? designId = null)
    {
        try
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid user authentication" });
            }

            var messages = await _messageService.GetConversation(currentUserId, userId, designId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error retrieving conversation: {ex.Message}" });
        }
    }

    [HttpGet]
    [HttpGet("user")]
    public async Task<IActionResult> GetUserMessages()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication" });
            }

            var messages = await _messageService.GetUserMessages(userId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error retrieving messages: {ex.Message}" });
        }
    }

    [HttpPut("{messageId}/read")]
    public async Task<IActionResult> MarkAsRead(int messageId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication" });
            }

            var success = await _messageService.MarkAsRead(messageId, userId);
            if (!success)
            {
                return NotFound(new { message = "Message not found or unauthorized" });
            }

            return Ok(new { message = "Message marked as read" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error marking message as read: {ex.Message}" });
        }
    }

    [HttpGet("unread/count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication" });
            }

            var count = await _messageService.GetUnreadCount(userId);
            return Ok(new { unreadCount = count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error getting unread count: {ex.Message}" });
        }
    }
}

public record SendMessageRequest(
    int ReceiverId,
    string Content,
    int? DesignId = null,
    int? QuoteId = null
);
