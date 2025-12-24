using MySql.Data.MySqlClient;

namespace Order.Service.Services;

public class OrderService : IOrderService
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly IHttpClientFactory _httpClientFactory;

    public OrderService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
        _httpClientFactory = httpClientFactory;
    }

    public async Task<OrderResult> CreateOrderAsync(int designId, int designerId, int supplierId, int quoteId, decimal amount)
    {
        try
        {
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO Orders (DesignId, DesignerId, SupplierId, QuoteId, OrderNumber, TotalAmount, Status, OrderDate, PaymentStatus)
                VALUES (@DesignId, @DesignerId, @SupplierId, @QuoteId, @OrderNumber, @TotalAmount, @Status, @OrderDate, @PaymentStatus);
                SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@DesignId", designId);
            command.Parameters.AddWithValue("@DesignerId", designerId);
            command.Parameters.AddWithValue("@SupplierId", supplierId);
            command.Parameters.AddWithValue("@QuoteId", quoteId);
            command.Parameters.AddWithValue("@OrderNumber", orderNumber);
            command.Parameters.AddWithValue("@TotalAmount", amount);
            command.Parameters.AddWithValue("@Status", "Pending");
            command.Parameters.AddWithValue("@OrderDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@PaymentStatus", "Pending");

            var orderId = Convert.ToInt32(await command.ExecuteScalarAsync());

            return new OrderResult(
                Success: true,
                OrderId: orderId,
                OrderNumber: orderNumber,
                Message: "Order created successfully"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Order creation error: {ex.Message}");
            return new OrderResult(
                Success: false,
                OrderId: null,
                OrderNumber: string.Empty,
                Message: $"Order creation failed: {ex.Message}"
            );
        }
    }

    public async Task<OrderDetails?> GetOrderByIdAsync(int orderId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT Id, DesignId, DesignerId, SupplierId, QuoteId, OrderNumber, TotalAmount, Status, OrderDate, PaymentStatus 
            FROM Orders 
            WHERE Id = @OrderId";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@OrderId", orderId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new OrderDetails(
                Id: reader.GetInt32(0),
                DesignId: reader.GetInt32(1),
                DesignerId: reader.GetInt32(2),
                SupplierId: reader.GetInt32(3),
                QuoteId: reader.GetInt32(4),
                OrderNumber: reader.GetString(5),
                TotalAmount: reader.GetDecimal(6),
                Status: reader.GetString(7),
                OrderDate: reader.GetDateTime(8),
                PaymentStatus: reader.GetString(9)
            );
        }

        return null;
    }

    public async Task<List<OrderRecord>> GetUserOrdersAsync(int userId, string userType)
    {
        var orders = new List<OrderRecord>();

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = userType == "Designer"
            ? @"SELECT Id, DesignId, DesignerId, SupplierId, QuoteId, OrderNumber, TotalAmount, Status, OrderDate, ShippingDate, DeliveryDate, ShippingAddress, TrackingNumber, PaymentStatus 
                FROM Orders WHERE DesignerId = @UserId ORDER BY OrderDate DESC"
            : @"SELECT Id, DesignId, DesignerId, SupplierId, QuoteId, OrderNumber, TotalAmount, Status, OrderDate, ShippingDate, DeliveryDate, ShippingAddress, TrackingNumber, PaymentStatus 
                FROM Orders WHERE SupplierId = @UserId ORDER BY OrderDate DESC";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@UserId", userId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            orders.Add(new OrderRecord(
                Id: reader.GetInt32(0),
                DesignId: reader.GetInt32(1),
                DesignerId: reader.GetInt32(2),
                SupplierId: reader.GetInt32(3),
                QuoteId: reader.GetInt32(4),
                OrderNumber: reader.GetString(5),
                TotalAmount: reader.GetDecimal(6),
                Status: reader.GetString(7),
                OrderDate: reader.GetDateTime(8),
                ShippingDate: reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                DeliveryDate: reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                ShippingAddress: reader.IsDBNull(11) ? null : reader.GetString(11),
                TrackingNumber: reader.IsDBNull(12) ? null : reader.GetString(12),
                PaymentStatus: reader.GetString(13)
            ));
        }

        return orders;
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Orders SET Status = @Status WHERE Id = @OrderId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@OrderId", orderId);
            
            await command.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update order status error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdatePaymentStatusAsync(int orderId, string paymentStatus)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Orders SET PaymentStatus = @PaymentStatus WHERE Id = @OrderId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
            command.Parameters.AddWithValue("@OrderId", orderId);
            
            await command.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update payment status error: {ex.Message}");
            return false;
        }
    }
}
