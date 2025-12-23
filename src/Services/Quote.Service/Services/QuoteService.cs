using Dressed.Shared.DTOs;
using Dressed.Shared.Models;
using MySql.Data.MySqlClient;

namespace Quote.Service.Services;

public class QuoteService : IQuoteService
{
    private readonly string _connectionString;

    public QuoteService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("Connection string not found");
    }

    public async Task<QuoteResponse?> CreateQuote(int supplierId, CreateQuoteRequest request)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO Quotes (DesignId, SupplierId, Price, DeliveryTimeInDays, QuoteText, TermsAndConditions, Status, CreatedAt)
                VALUES (@DesignId, @SupplierId, @Price, @DeliveryTimeInDays, @QuoteText, @TermsAndConditions, @Status, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@DesignId", request.DesignId);
            cmd.Parameters.AddWithValue("@SupplierId", supplierId);
            cmd.Parameters.AddWithValue("@Price", request.Price);
            cmd.Parameters.AddWithValue("@DeliveryTimeInDays", request.DeliveryTimeInDays);
            cmd.Parameters.AddWithValue("@QuoteText", request.QuoteText);
            cmd.Parameters.AddWithValue("@TermsAndConditions", request.TermsAndConditions);
            cmd.Parameters.AddWithValue("@Status", (int)QuoteStatus.Submitted);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            var quoteId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return await GetQuote(quoteId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create quote error: {ex.Message}");
            return null;
        }
    }

    public async Task<QuoteResponse?> GetQuote(int id)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT q.Id, q.DesignId, q.SupplierId, q.Price, q.Currency, q.DeliveryTimeInDays, 
                       q.QuoteText, q.TermsAndConditions, q.Status, q.CreatedAt,
                       s.CompanyName
                FROM Quotes q
                LEFT JOIN Suppliers s ON q.SupplierId = s.Id
                WHERE q.Id = @Id";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new QuoteResponse
            {
                Id = reader.GetInt32(0),
                DesignId = reader.GetInt32(1),
                SupplierId = reader.GetInt32(2),
                Price = reader.GetDecimal(3),
                Currency = reader.GetString(4),
                DeliveryTimeInDays = reader.GetInt32(5),
                QuoteText = reader.GetString(6),
                TermsAndConditions = reader.GetString(7),
                Status = ((QuoteStatus)reader.GetInt32(8)).ToString(),
                CreatedAt = reader.GetDateTime(9),
                SupplierName = reader.IsDBNull(10) ? "" : reader.GetString(10)
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get quote error: {ex.Message}");
            return null;
        }
    }

    public async Task<List<QuoteResponse>> GetQuotesByDesign(int designId)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT q.Id, q.DesignId, q.SupplierId, q.Price, q.Currency, q.DeliveryTimeInDays, 
                       q.QuoteText, q.TermsAndConditions, q.Status, q.CreatedAt,
                       s.CompanyName
                FROM Quotes q
                LEFT JOIN Suppliers s ON q.SupplierId = s.Id
                WHERE q.DesignId = @DesignId
                ORDER BY q.CreatedAt DESC";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@DesignId", designId);

            var quotes = new List<QuoteResponse>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                quotes.Add(new QuoteResponse
                {
                    Id = reader.GetInt32(0),
                    DesignId = reader.GetInt32(1),
                    SupplierId = reader.GetInt32(2),
                    Price = reader.GetDecimal(3),
                    Currency = reader.GetString(4),
                    DeliveryTimeInDays = reader.GetInt32(5),
                    QuoteText = reader.GetString(6),
                    TermsAndConditions = reader.GetString(7),
                    Status = ((QuoteStatus)reader.GetInt32(8)).ToString(),
                    CreatedAt = reader.GetDateTime(9),
                    SupplierName = reader.IsDBNull(10) ? "" : reader.GetString(10)
                });
            }

            return quotes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get quotes by design error: {ex.Message}");
            return new List<QuoteResponse>();
        }
    }

    public async Task<List<QuoteResponse>> GetQuotesBySupplier(int supplierId)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT q.Id, q.DesignId, q.SupplierId, q.Price, q.Currency, q.DeliveryTimeInDays, 
                       q.QuoteText, q.TermsAndConditions, q.Status, q.CreatedAt,
                       s.CompanyName
                FROM Quotes q
                LEFT JOIN Suppliers s ON q.SupplierId = s.Id
                WHERE q.SupplierId = @SupplierId
                ORDER BY q.CreatedAt DESC";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@SupplierId", supplierId);

            var quotes = new List<QuoteResponse>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                quotes.Add(new QuoteResponse
                {
                    Id = reader.GetInt32(0),
                    DesignId = reader.GetInt32(1),
                    SupplierId = reader.GetInt32(2),
                    Price = reader.GetDecimal(3),
                    Currency = reader.GetString(4),
                    DeliveryTimeInDays = reader.GetInt32(5),
                    QuoteText = reader.GetString(6),
                    TermsAndConditions = reader.GetString(7),
                    Status = ((QuoteStatus)reader.GetInt32(8)).ToString(),
                    CreatedAt = reader.GetDateTime(9),
                    SupplierName = reader.IsDBNull(10) ? "" : reader.GetString(10)
                });
            }

            return quotes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get quotes by supplier error: {ex.Message}");
            return new List<QuoteResponse>();
        }
    }

    public async Task<bool> UpdateQuoteStatus(int id, string status)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var statusEnum = Enum.Parse<QuoteStatus>(status, true);

            var query = "UPDATE Quotes SET Status = @Status, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Status", (int)statusEnum);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@Id", id);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update quote status error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteQuote(int id)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Quotes WHERE Id = @Id";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete quote error: {ex.Message}");
            return false;
        }
    }
}
