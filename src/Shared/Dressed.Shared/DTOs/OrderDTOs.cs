namespace Dressed.Shared.DTOs;

public class CreateOrderRequest
{
    public int QuoteId { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
}

public class OrderResponse
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int DesignId { get; set; }
    public string DesignTitle { get; set; } = string.Empty;
    public int DesignerId { get; set; }
    public string DesignerName { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
}
