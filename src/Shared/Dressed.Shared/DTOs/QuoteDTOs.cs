namespace Dressed.Shared.DTOs;

public class CreateQuoteRequest
{
    public int DesignId { get; set; }
    public decimal Price { get; set; }
    public int DeliveryTimeInDays { get; set; }
    public string QuoteText { get; set; } = string.Empty;
    public string TermsAndConditions { get; set; } = string.Empty;
}

public class QuoteResponse
{
    public int Id { get; set; }
    public int DesignId { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public int DeliveryTimeInDays { get; set; }
    public string QuoteText { get; set; } = string.Empty;
    public string TermsAndConditions { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
