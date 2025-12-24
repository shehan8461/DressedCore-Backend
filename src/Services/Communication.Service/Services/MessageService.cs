using Dressed.Shared.DTOs;
using Dressed.Shared.Models;
using MySql.Data.MySqlClient;

namespace Communication.Service.Services;

public class MessageService : IMessageService
{
    private readonly string _connectionString;

    public MessageService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("Connection string not found");
    }

    public async Task<int> SendMessage(int senderId, int receiverId, string content, int? designId = null, int? quoteId = null)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO Messages (SenderId, ReceiverId, Content, DesignId, QuoteId, SentAt, IsRead)
                VALUES (@SenderId, @ReceiverId, @Content, @DesignId, @QuoteId, @SentAt, 0);
                SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@SenderId", senderId);
            cmd.Parameters.AddWithValue("@ReceiverId", receiverId);
            cmd.Parameters.AddWithValue("@Content", content);
            cmd.Parameters.AddWithValue("@DesignId", designId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@QuoteId", quoteId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SentAt", DateTime.UtcNow);

            var messageId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return messageId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Send message error: {ex.Message}");
            return -1;
        }
    }

    public async Task<List<MessageResponse>> GetConversation(int userId, int otherUserId, int? designId = null)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT Id, SenderId, ReceiverId, DesignId, QuoteId, Content, SentAt, IsRead
                FROM Messages
                WHERE ((SenderId = @UserId AND ReceiverId = @OtherUserId) 
                    OR (SenderId = @OtherUserId AND ReceiverId = @UserId))";

            if (designId.HasValue)
            {
                query += " AND DesignId = @DesignId";
            }

            query += " ORDER BY SentAt ASC";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@OtherUserId", otherUserId);
            if (designId.HasValue)
            {
                cmd.Parameters.AddWithValue("@DesignId", designId.Value);
            }

            var messages = new List<MessageResponse>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(new MessageResponse(
                    MessageId: reader.GetInt32(0),
                    SenderId: reader.GetInt32(1),
                    ReceiverId: reader.GetInt32(2),
                    Content: reader.GetString(5),
                    DesignId: reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    QuoteId: reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    IsRead: reader.GetBoolean(7),
                    CreatedAt: reader.GetDateTime(6)
                ));
            }

            return messages;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get conversation error: {ex.Message}");
            return new List<MessageResponse>();
        }
    }

    public async Task<List<MessageResponse>> GetUserMessages(int userId)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT Id, SenderId, ReceiverId, DesignId, QuoteId, Content, SentAt, IsRead
                FROM Messages
                WHERE SenderId = @UserId OR ReceiverId = @UserId
                ORDER BY SentAt DESC
                LIMIT 100";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            var messages = new List<MessageResponse>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(new MessageResponse(
                    MessageId: reader.GetInt32(0),
                    SenderId: reader.GetInt32(1),
                    ReceiverId: reader.GetInt32(2),
                    Content: reader.GetString(5),
                    DesignId: reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    QuoteId: reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    IsRead: reader.GetBoolean(7),
                    CreatedAt: reader.GetDateTime(6)
                ));
            }

            return messages;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get user messages error: {ex.Message}");
            return new List<MessageResponse>();
        }
    }

    public async Task<bool> MarkAsRead(int messageId, int userId)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Messages SET IsRead = 1 WHERE Id = @MessageId AND ReceiverId = @UserId";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@MessageId", messageId);
            cmd.Parameters.AddWithValue("@UserId", userId);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Mark as read error: {ex.Message}");
            return false;
        }
    }

    public async Task<int> GetUnreadCount(int userId)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM Messages WHERE ReceiverId = @UserId AND IsRead = 0";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get unread count error: {ex.Message}");
            return 0;
        }
    }
}
