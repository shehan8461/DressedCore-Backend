using Dressed.Shared.DTOs;
using Dressed.Shared.Models;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace Design.Service.Services;

public class DesignService : IDesignService
{
    private readonly string _connectionString;

    public DesignService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("Connection string not found");
    }

    public async Task<DesignResponse?> CreateDesign(int designerId, CreateDesignRequest request)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var category = Enum.Parse<ClothingCategory>(request.Category, true);
            var fileUrlsJson = JsonSerializer.Serialize(request.FileUrls);

            var query = @"
                INSERT INTO Designs (DesignerId, Title, Description, Category, FileUrls, Status, CreatedAt, Deadline, Quantity, Specifications)
                VALUES (@DesignerId, @Title, @Description, @Category, @FileUrls, @Status, @CreatedAt, @Deadline, @Quantity, @Specifications);
                SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@DesignerId", designerId);
            cmd.Parameters.AddWithValue("@Title", request.Title);
            cmd.Parameters.AddWithValue("@Description", request.Description);
            cmd.Parameters.AddWithValue("@Category", (int)category);
            cmd.Parameters.AddWithValue("@FileUrls", fileUrlsJson);
            cmd.Parameters.AddWithValue("@Status", (int)DesignStatus.Published);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@Deadline", request.Deadline ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Quantity", request.Quantity);
            cmd.Parameters.AddWithValue("@Specifications", request.Specifications);

            var designId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return await GetDesign(designId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create design error: {ex.Message}");
            return null;
        }
    }

    public async Task<DesignResponse?> GetDesign(int id)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT d.Id, d.DesignerId, d.Title, d.Description, d.Category, d.FileUrls, d.Status, 
                       d.CreatedAt, d.Deadline, d.Quantity, d.Specifications,
                       des.CompanyName,
                       (SELECT COUNT(*) FROM Quotes q WHERE q.DesignId = d.Id) as QuoteCount
                FROM Designs d
                LEFT JOIN Designers des ON d.DesignerId = des.Id
                WHERE d.Id = @Id";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            var fileUrlsJson = reader.GetString(5);
            var fileUrls = JsonSerializer.Deserialize<List<string>>(fileUrlsJson) ?? new List<string>();

            return new DesignResponse
            {
                Id = reader.GetInt32(0),
                DesignerId = reader.GetInt32(1),
                Title = reader.GetString(2),
                Description = reader.GetString(3),
                Category = ((ClothingCategory)reader.GetInt32(4)).ToString(),
                FileUrls = fileUrls,
                Status = ((DesignStatus)reader.GetInt32(6)).ToString(),
                CreatedAt = reader.GetDateTime(7),
                Deadline = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                Quantity = reader.GetInt32(9),
                Specifications = reader.GetString(10),
                DesignerName = reader.IsDBNull(11) ? "" : reader.GetString(11),
                QuoteCount = reader.GetInt32(12)
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get design error: {ex.Message}");
            return null;
        }
    }

    public async Task<List<DesignResponse>> GetDesignerDesigns(int designerId)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT d.Id, d.DesignerId, d.Title, d.Description, d.Category, d.FileUrls, d.Status, 
                       d.CreatedAt, d.Deadline, d.Quantity, d.Specifications,
                       des.CompanyName,
                       (SELECT COUNT(*) FROM Quotes q WHERE q.DesignId = d.Id) as QuoteCount
                FROM Designs d
                LEFT JOIN Designers des ON d.DesignerId = des.Id
                WHERE d.DesignerId = @DesignerId
                ORDER BY d.CreatedAt DESC";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@DesignerId", designerId);

            var designs = new List<DesignResponse>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var fileUrlsJson = reader.GetString(5);
                var fileUrls = JsonSerializer.Deserialize<List<string>>(fileUrlsJson) ?? new List<string>();

                designs.Add(new DesignResponse
                {
                    Id = reader.GetInt32(0),
                    DesignerId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = reader.GetString(3),
                    Category = ((ClothingCategory)reader.GetInt32(4)).ToString(),
                    FileUrls = fileUrls,
                    Status = ((DesignStatus)reader.GetInt32(6)).ToString(),
                    CreatedAt = reader.GetDateTime(7),
                    Deadline = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    Quantity = reader.GetInt32(9),
                    Specifications = reader.GetString(10),
                    DesignerName = reader.IsDBNull(11) ? "" : reader.GetString(11),
                    QuoteCount = reader.GetInt32(12)
                });
            }

            return designs;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get designer designs error: {ex.Message}");
            return new List<DesignResponse>();
        }
    }

    public async Task<List<DesignResponse>> GetAllDesigns(string? category = null)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT d.Id, d.DesignerId, d.Title, d.Description, d.Category, d.FileUrls, d.Status, 
                       d.CreatedAt, d.Deadline, d.Quantity, d.Specifications,
                       des.CompanyName,
                       (SELECT COUNT(*) FROM Quotes q WHERE q.DesignId = d.Id) as QuoteCount
                FROM Designs d
                LEFT JOIN Designers des ON d.DesignerId = des.Id
                WHERE d.Status IN (2, 3)"; // Published or QuotingOpen

            if (!string.IsNullOrEmpty(category))
            {
                var categoryEnum = Enum.Parse<ClothingCategory>(category, true);
                query += " AND d.Category = @Category";
            }

            query += " ORDER BY d.CreatedAt DESC";

            using var cmd = new MySqlCommand(query, connection);
            if (!string.IsNullOrEmpty(category))
            {
                var categoryEnum = Enum.Parse<ClothingCategory>(category, true);
                cmd.Parameters.AddWithValue("@Category", (int)categoryEnum);
            }

            var designs = new List<DesignResponse>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var fileUrlsJson = reader.GetString(5);
                var fileUrls = JsonSerializer.Deserialize<List<string>>(fileUrlsJson) ?? new List<string>();

                designs.Add(new DesignResponse
                {
                    Id = reader.GetInt32(0),
                    DesignerId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = reader.GetString(3),
                    Category = ((ClothingCategory)reader.GetInt32(4)).ToString(),
                    FileUrls = fileUrls,
                    Status = ((DesignStatus)reader.GetInt32(6)).ToString(),
                    CreatedAt = reader.GetDateTime(7),
                    Deadline = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    Quantity = reader.GetInt32(9),
                    Specifications = reader.GetString(10),
                    DesignerName = reader.IsDBNull(11) ? "" : reader.GetString(11),
                    QuoteCount = reader.GetInt32(12)
                });
            }

            return designs;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get all designs error: {ex.Message}");
            return new List<DesignResponse>();
        }
    }

    public async Task<bool> UpdateDesignStatus(int id, string status)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var statusEnum = Enum.Parse<DesignStatus>(status, true);

            var query = "UPDATE Designs SET Status = @Status WHERE Id = @Id";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Status", (int)statusEnum);
            cmd.Parameters.AddWithValue("@Id", id);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update design status error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteDesign(int id)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Designs WHERE Id = @Id";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete design error: {ex.Message}");
            return false;
        }
    }
}
