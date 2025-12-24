using MySql.Data.MySqlClient;

namespace Payment.Service.Services;

public class TransactionService : ITransactionService
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public TransactionService(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    public async Task<int> CreateTransactionAsync(int paymentId, string type, decimal amount, string status)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            INSERT INTO Transactions (PaymentId, Type, Amount, Status, CreatedAt)
            VALUES (@PaymentId, @Type, @Amount, @Status, @CreatedAt);
            SELECT LAST_INSERT_ID();";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@PaymentId", paymentId);
        command.Parameters.AddWithValue("@Type", type);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@Status", status);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<bool> UpdateTransactionStatusAsync(int transactionId, string status)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Transactions SET Status = @Status WHERE TransactionId = @TransactionId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@TransactionId", transactionId);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Transaction update error: {ex.Message}");
            return false;
        }
    }

    public async Task<List<TransactionRecord>> GetPaymentTransactionsAsync(int paymentId)
    {
        var transactions = new List<TransactionRecord>();

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT TransactionId, PaymentId, Type, Amount, Status, CreatedAt 
            FROM Transactions 
            WHERE PaymentId = @PaymentId 
            ORDER BY CreatedAt DESC";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@PaymentId", paymentId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            transactions.Add(new TransactionRecord(
                TransactionId: reader.GetInt32(0),
                PaymentId: reader.GetInt32(1),
                Type: reader.GetString(2),
                Amount: reader.GetDecimal(3),
                Status: reader.GetString(4),
                CreatedAt: reader.GetDateTime(5)
            ));
        }

        return transactions;
    }
}
