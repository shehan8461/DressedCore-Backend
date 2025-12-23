namespace Dressed.Shared.Models;

public class Quote
{
    public int Id { get; set; }
    public int DesignId { get; set; }
    public int SupplierId { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public int DeliveryTimeInDays { get; set; }
    public string QuoteText { get; set; } = string.Empty;
    public string TermsAndConditions { get; set; } = string.Empty;
    public QuoteStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum QuoteStatus
{
    Submitted = 1,
    UnderNegotiation = 2,
    Accepted = 3,
    Rejected = 4,
    Expired = 5
}
