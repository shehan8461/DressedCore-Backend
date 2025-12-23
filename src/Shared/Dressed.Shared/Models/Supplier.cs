namespace Dressed.Shared.Models;

public class Supplier
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ManufacturingCapabilities { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ClothingCategory> SubscribedCategories { get; set; } = new();
}
