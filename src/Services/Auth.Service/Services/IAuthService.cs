using Dressed.Shared.DTOs;

namespace Auth.Service.Services;

public interface IAuthService
{
    Task<AuthResponse?> Register(RegisterRequest request);
    Task<AuthResponse?> Login(LoginRequest request);
    Task<bool> ValidateToken(string token);
}
