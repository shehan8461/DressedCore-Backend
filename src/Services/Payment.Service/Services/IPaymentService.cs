namespace Payment.Service.Services;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(int orderId, int userId, decimal amount, string paymentMethod);
    Task<PaymentStatus> GetPaymentStatusAsync(int paymentId);
    Task<List<PaymentRecord>> GetUserPaymentsAsync(int userId);
    Task<bool> RefundPaymentAsync(int paymentId, decimal amount, string reason);
    Task<decimal> CalculatePlatformFeeAsync(decimal amount);
}

public interface ITransactionService
{
    Task<int> CreateTransactionAsync(int paymentId, string type, decimal amount, string status);
    Task<bool> UpdateTransactionStatusAsync(int transactionId, string status);
    Task<List<TransactionRecord>> GetPaymentTransactionsAsync(int paymentId);
}

public record PaymentResult(
    bool Success,
    int? PaymentId,
    string? TransactionId,
    string Message
);

public record PaymentStatus(
    int PaymentId,
    string Status,
    decimal Amount,
    DateTime CreatedAt,
    string? TransactionId
);

public record PaymentRecord(
    int PaymentId,
    int OrderId,
    decimal Amount,
    decimal PlatformFee,
    string Status,
    string PaymentMethod,
    DateTime CreatedAt,
    string? TransactionId
);

public record TransactionRecord(
    int TransactionId,
    int PaymentId,
    string Type,
    decimal Amount,
    string Status,
    DateTime CreatedAt
);
