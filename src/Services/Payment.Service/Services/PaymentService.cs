using MySql.Data.MySqlClient;

namespace Payment.Service.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly ITransactionService _transactionService;

    public PaymentService(IConfiguration configuration, ITransactionService transactionService)
    {
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
        _transactionService = transactionService;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(int orderId, int userId, decimal amount, string paymentMethod)
    {
        try
        {
            // Calculate platform fee
            var platformFee = await CalculatePlatformFeeAsync(amount);
            var totalAmount = amount + platformFee;

            // Here you would integrate with actual payment gateway (Stripe, PayPal, etc.)
            // For now, we'll simulate a successful payment
            
            var paymentStatus = "Completed"; // or "Failed" based on payment gateway response
            var transactionId = Guid.NewGuid().ToString(); // Mock transaction ID

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO Payments (UserId, OrderId, Amount, PlatformFee, Status, PaymentMethod, TransactionId, CreatedAt)
                VALUES (@UserId, @OrderId, @Amount, @PlatformFee, @Status, @PaymentMethod, @TransactionId, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@PlatformFee", platformFee);
            command.Parameters.AddWithValue("@Status", paymentStatus);
            command.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
            command.Parameters.AddWithValue("@TransactionId", transactionId);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            var paymentId = Convert.ToInt32(await command.ExecuteScalarAsync());

            // Create transaction record
            await _transactionService.CreateTransactionAsync(paymentId, "Payment", totalAmount, paymentStatus);

            return new PaymentResult(
                Success: true,
                PaymentId: paymentId,
                TransactionId: transactionId,
                Message: "Payment processed successfully"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Payment processing error: {ex.Message}");
            return new PaymentResult(
                Success: false,
                PaymentId: null,
                TransactionId: null,
                Message: $"Payment failed: {ex.Message}"
            );
        }
    }

    public async Task<PaymentStatus> GetPaymentStatusAsync(int paymentId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "SELECT PaymentId, Status, Amount, CreatedAt, TransactionId FROM Payments WHERE PaymentId = @PaymentId";
        
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@PaymentId", paymentId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new PaymentStatus(
                PaymentId: reader.GetInt32(0),
                Status: reader.GetString(1),
                Amount: reader.GetDecimal(2),
                CreatedAt: reader.GetDateTime(3),
                TransactionId: reader.IsDBNull(4) ? null : reader.GetString(4)
            );
        }

        throw new Exception("Payment not found");
    }

    public async Task<List<PaymentRecord>> GetUserPaymentsAsync(int userId)
    {
        var payments = new List<PaymentRecord>();

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT PaymentId, OrderId, Amount, PlatformFee, Status, PaymentMethod, CreatedAt, TransactionId 
            FROM Payments 
            WHERE UserId = @UserId 
            ORDER BY CreatedAt DESC 
            LIMIT 100";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@UserId", userId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            payments.Add(new PaymentRecord(
                PaymentId: reader.GetInt32(0),
                OrderId: reader.GetInt32(1),
                Amount: reader.GetDecimal(2),
                PlatformFee: reader.GetDecimal(3),
                Status: reader.GetString(4),
                PaymentMethod: reader.GetString(5),
                CreatedAt: reader.GetDateTime(6),
                TransactionId: reader.IsDBNull(7) ? null : reader.GetString(7)
            ));
        }

        return payments;
    }

    public async Task<bool> RefundPaymentAsync(int paymentId, decimal amount, string reason)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            // Update payment status
            var query = "UPDATE Payments SET Status = 'Refunded' WHERE PaymentId = @PaymentId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@PaymentId", paymentId);
            await command.ExecuteNonQueryAsync();

            // Create refund transaction
            await _transactionService.CreateTransactionAsync(paymentId, "Refund", amount, "Completed");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Refund error: {ex.Message}");
            return false;
        }
    }

    public async Task<decimal> CalculatePlatformFeeAsync(decimal amount)
    {
        // Platform fee: 10% of the amount
        var platformFee = Math.Round(amount * 0.10m, 2);
        return platformFee;
    }

    public async Task<List<TransactionRecord>> GetPaymentTransactionsAsync(int paymentId)
    {
        return await _transactionService.GetPaymentTransactionsAsync(paymentId);
    }
}
