namespace Dressed.Shared.Models;

public class Order
{
    public int Id { get; set; }
    public int DesignId { get; set; }
    public int DesignerId { get; set; }
    public int SupplierId { get; set; }
    public int QuoteId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
    public PaymentStatus PaymentStatus { get; set; }
}

public enum OrderStatus
{
    Placed = 1,
    Confirmed = 2,
    InProduction = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Disputed = 7
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Refunded = 3,
    Failed = 4
}
