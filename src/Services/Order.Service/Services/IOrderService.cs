namespace Order.Service.Services;

public interface IOrderService
{
    Task<OrderResult> CreateOrderAsync(int designId, int designerId, int supplierId, int quoteId, decimal amount);
    Task<OrderDetails?> GetOrderByIdAsync(int orderId);
    Task<List<OrderRecord>> GetUserOrdersAsync(int userId, string userType);
    Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    Task<bool> UpdatePaymentStatusAsync(int orderId, string paymentStatus);
}

public record OrderResult(
    bool Success,
    int? OrderId,
    string OrderNumber,
    string Message
);

public record OrderDetails(
    int Id,
    int DesignId,
    int DesignerId,
    int SupplierId,
    int QuoteId,
    string OrderNumber,
    decimal TotalAmount,
    string Status,
    DateTime OrderDate,
    string PaymentStatus
);

public record OrderRecord(
    int Id,
    int DesignId,
    int DesignerId,
    int SupplierId,
    int QuoteId,
    string OrderNumber,
    decimal TotalAmount,
    string Status,
    DateTime OrderDate,
    DateTime? ShippingDate,
    DateTime? DeliveryDate,
    string? ShippingAddress,
    string? TrackingNumber,
    string PaymentStatus
);
