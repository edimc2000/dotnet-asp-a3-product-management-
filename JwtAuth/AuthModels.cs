// API assistance was used on this class, both logic and documentation 

using static ProductManagement.Helper.Helper;

namespace ProductManagement.JwtAuth;

/// <summary>Represents a login request with username and password.</summary>
public record LoginRequest(string Username, string Password);

/// <summary>Represents a login response with JWT token and message.</summary>
public record LoginResponse(string Token, string Message);

/// <summary>Represents a user model for authentication.</summary>
public class UserModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

/// <summary>Contains JWT configuration settings loaded from configuration.</summary>
public class JwtSettings
{
    public string?  Key { get; set; } = DevConfiguration["Jwt:Key"];
    public string? Issuer { get; set; } = DevConfiguration["Jwt:Issuer"];
    public string? Audience { get; set; } = DevConfiguration["Jwt:Audience"];
    public int ExpiryInMinutes { get; set; } = int.TryParse(DevConfiguration["Jwt_settings:ExpiryInMinutes"], 
        out int expiry) ? expiry : 120;
}

/// <summary>Interface for JWT token generation services.</summary>
public interface ITokenService
{
    string GenerateToken(UserModel user);
}