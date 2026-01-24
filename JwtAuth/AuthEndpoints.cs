// API assistance was used on this class, both logic and documentation 


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ProductManagement.EntityModels.JwtAuth;

namespace ProductManagement.JwtAuth;

/// <summary>Authentication endpoints for JWT token operations.</summary>
public class AuthEndpoints
{
    /// <summary>Generates a valid JWT token for authenticated users.</summary>
    /// <param name="request">The login request containing credentials.</param>
    /// <param name="tokenService">Service for JWT token generation.</param>
    /// <param name="authService">Service for user authentication.</param>
    /// <param name="jwtSettings">Configuration settings for JWT.</param>
    /// <returns>An IResult containing the token or authentication error.</returns>
    public static IResult GetValidToken
    (
        LoginRequest request,
        ITokenService tokenService,
        IAuthService authService,
        JwtSettings jwtSettings
    )
    {
        if (string.IsNullOrEmpty(request.Username) ||
            string.IsNullOrEmpty(request.Password))
            return Results.BadRequest("Username and password are required");

        UserModel? user = authService.Authenticate(request.Username, request.Password);

        if (user == null) return Results.Unauthorized();

        string token = tokenService.GenerateToken(user);

        return Results.Ok(new LoginResponse(
            token,
            $"Welcome {user.Username}! Token valid for {jwtSettings.ExpiryInMinutes} minutes."
        ));
    }

    /// <summary>Validates the current JWT token and returns its details.</summary>
    /// <param name="context">The HTTP context containing the authorization header.</param>
    /// <returns>An IResult containing token validation details.</returns>
    public static IResult GetTokenValidity
        (HttpContext context)
    {
        string? authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        string? ValidUntil = context.User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        int.TryParse(ValidUntil, out int parsedId);
        DateTime formattedValidity =
            DateTimeOffset.FromUnixTimeSeconds(parsedId).LocalDateTime;

        return Results.Ok(new
        {
            Message = "Token is valid!",
            TokenPresent = !string.IsNullOrEmpty(authHeader),
            User = context.User.Identity?.Name,
            Role = context.User.FindFirst(ClaimTypes.Role)?.Value,
            //ValidUntil = context.User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value
            ValidUntil = $"{formattedValidity}"
        });
    }
}