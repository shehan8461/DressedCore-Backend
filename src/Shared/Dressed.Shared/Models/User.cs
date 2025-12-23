namespace Dressed.Shared.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public enum UserType
{
    Designer = 1,
    Supplier = 2,
    Admin = 3
}
