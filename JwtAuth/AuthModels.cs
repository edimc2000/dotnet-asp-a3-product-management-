using static ProductManagement.Helper.Helper;

namespace ProductManagement.JwtAuth;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, string Message);

public class UserModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class JwtSettings
{
    public string?  Key { get; set; } = DevConfiguration["Jwt:Key"];
    public string? Issuer { get; set; } = DevConfiguration["Jwt:Issuer"];
    public string? Audience { get; set; } = DevConfiguration["Jwt:Audience"];
    public int ExpiryInMinutes { get; set; } = int.TryParse(DevConfiguration["Jwt_settings:ExpiryInMinutes"], 
        out int expiry) ? expiry : 120;
}

public interface ITokenService
{
    string GenerateToken(UserModel user);
}