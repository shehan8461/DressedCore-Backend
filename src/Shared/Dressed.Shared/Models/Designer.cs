namespace Dressed.Shared.Models;

public class Designer
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}
