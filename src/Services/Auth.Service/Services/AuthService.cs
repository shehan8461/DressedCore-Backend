using BCrypt.Net;
using Dressed.Shared.DTOs;
using Dressed.Shared.Models;
using Dressed.Shared.Utilities;
using MySql.Data.MySqlClient;

namespace Auth.Service.Services;

public class AuthService : IAuthService
{
    private readonly string _connectionString;
    private readonly string _jwtSecret;

    public AuthService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("Connection string not found");
        _jwtSecret = configuration["Jwt:Secret"] 
            ?? throw new ArgumentNullException("JWT secret not found");
    }

    public async Task<AuthResponse?> Register(RegisterRequest request)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            // Check if user already exists
            var checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            using var checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@Email", request.Email);
            var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;

            if (exists)
                return null;

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Determine user type
            var userType = request.UserType.ToLower() == "designer" ? UserType.Designer : UserType.Supplier;

            // Insert user
            var insertUserQuery = @"
                INSERT INTO Users (Email, PasswordHash, FirstName, LastName, UserType, CreatedAt, IsActive)
                VALUES (@Email, @PasswordHash, @FirstName, @LastName, @UserType, @CreatedAt, @IsActive);
                SELECT LAST_INSERT_ID();";

            using var insertCmd = new MySqlCommand(insertUserQuery, connection);
            insertCmd.Parameters.AddWithValue("@Email", request.Email);
            insertCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            insertCmd.Parameters.AddWithValue("@FirstName", request.FirstName);
            insertCmd.Parameters.AddWithValue("@LastName", request.LastName);
            insertCmd.Parameters.AddWithValue("@UserType", (int)userType);
            insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            insertCmd.Parameters.AddWithValue("@IsActive", true);

            var userId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());

            // Insert designer or supplier profile
            if (userType == UserType.Designer)
            {
                var designerQuery = @"
                    INSERT INTO Designers (UserId, CompanyName, ContactNumber, Address, Rating, CreatedAt)
                    VALUES (@UserId, @CompanyName, @ContactNumber, @Address, 0, @CreatedAt)";
                
                using var designerCmd = new MySqlCommand(designerQuery, connection);
                designerCmd.Parameters.AddWithValue("@UserId", userId);
                designerCmd.Parameters.AddWithValue("@CompanyName", request.CompanyName);
                designerCmd.Parameters.AddWithValue("@ContactNumber", request.ContactNumber);
                designerCmd.Parameters.AddWithValue("@Address", request.Address);
                designerCmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                await designerCmd.ExecuteNonQueryAsync();
            }
            else
            {
                var supplierQuery = @"
                    INSERT INTO Suppliers (UserId, CompanyName, ContactNumber, Address, Rating, CreatedAt)
                    VALUES (@UserId, @CompanyName, @ContactNumber, @Address, 0, @CreatedAt)";
                
                using var supplierCmd = new MySqlCommand(supplierQuery, connection);
                supplierCmd.Parameters.AddWithValue("@UserId", userId);
                supplierCmd.Parameters.AddWithValue("@CompanyName", request.CompanyName);
                supplierCmd.Parameters.AddWithValue("@ContactNumber", request.ContactNumber);
                supplierCmd.Parameters.AddWithValue("@Address", request.Address);
                supplierCmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                await supplierCmd.ExecuteNonQueryAsync();
            }

            // Generate JWT token
            var token = JwtHelper.GenerateToken(userId, request.Email, userType.ToString(), _jwtSecret);

            return new AuthResponse
            {
                Token = token,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserType = userType.ToString(),
                UserId = userId
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration error: {ex.Message}");
            return null;
        }
    }

    public async Task<AuthResponse?> Login(LoginRequest request)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT Id, Email, PasswordHash, FirstName, LastName, UserType FROM Users WHERE Email = @Email AND IsActive = 1";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Email", request.Email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            var userId = reader.GetInt32(0);
            var email = reader.GetString(1);
            var passwordHash = reader.GetString(2);
            var firstName = reader.GetString(3);
            var lastName = reader.GetString(4);
            var userType = (UserType)reader.GetInt32(5);

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, passwordHash))
                return null;

            // Generate JWT token
            var token = JwtHelper.GenerateToken(userId, email, userType.ToString(), _jwtSecret);

            return new AuthResponse
            {
                Token = token,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                UserType = userType.ToString(),
                UserId = userId
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> ValidateToken(string token)
    {
        try
        {
            var principal = JwtHelper.ValidateToken(token, _jwtSecret);
            return principal != null;
        }
        catch
        {
            return false;
        }
    }
}
