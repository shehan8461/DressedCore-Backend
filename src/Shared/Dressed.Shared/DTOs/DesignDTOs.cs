namespace Dressed.Shared.DTOs;

public class CreateDesignRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> FileUrls { get; set; } = new();
    public DateTime? Deadline { get; set; }
    public int Quantity { get; set; }
    public string Specifications { get; set; } = string.Empty;
}

public class DesignResponse
{
    public int Id { get; set; }
    public int DesignerId { get; set; }
    public string DesignerName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> FileUrls { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? Deadline { get; set; }
    public int Quantity { get; set; }
    public string Specifications { get; set; } = string.Empty;
    public int QuoteCount { get; set; }
}
