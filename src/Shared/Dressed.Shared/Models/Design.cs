namespace Dressed.Shared.Models;

public class Design
{
    public int Id { get; set; }
    public int DesignerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ClothingCategory Category { get; set; }
    public List<string> FileUrls { get; set; } = new();
    public DesignStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? Deadline { get; set; }
    public int Quantity { get; set; }
    public string Specifications { get; set; } = string.Empty;
}

public enum ClothingCategory
{
    Men = 1,
    Women = 2,
    Boy = 3,
    Girl = 4,
    Unisex = 5
}

public enum DesignStatus
{
    Draft = 1,
    Published = 2,
    QuotingOpen = 3,
    QuotingClosed = 4,
    Ordered = 5,
    Completed = 6,
    Cancelled = 7
}
