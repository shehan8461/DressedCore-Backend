using Dressed.Shared.DTOs;

namespace Communication.Service.Services;

public interface IMessageService
{
    Task<int> SendMessage(int senderId, int receiverId, string content, int? designId = null, int? quoteId = null);
    Task<List<MessageResponse>> GetConversation(int userId, int otherUserId, int? designId = null);
    Task<List<MessageResponse>> GetUserMessages(int userId);
    Task<bool> MarkAsRead(int messageId, int userId);
    Task<int> GetUnreadCount(int userId);
}
