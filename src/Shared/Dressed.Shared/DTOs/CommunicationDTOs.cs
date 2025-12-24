namespace Dressed.Shared.DTOs;

// Message DTOs
public record SendMessageRequest(
    int ReceiverId,
    string Content,
    int? DesignId = null,
    int? QuoteId = null
);

public record MessageResponse(
    int MessageId,
    int SenderId,
    int ReceiverId,
    string Content,
    int? DesignId,
    int? QuoteId,
    bool IsRead,
    DateTime CreatedAt
);

// Payment DTOs
public record ProcessPaymentRequest(
    int OrderId,
    decimal Amount,
    string PaymentMethod
);

public record PaymentResponse(
    int PaymentId,
    int OrderId,
    decimal Amount,
    decimal PlatformFee,
    string Status,
    string PaymentMethod,
    string? TransactionId,
    DateTime CreatedAt
);

public record CalculateFeeRequest(
    decimal Amount
);

public record CalculateFeeResponse(
    decimal PlatformFee,
    decimal TotalAmount
);
