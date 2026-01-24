using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ProductManagement.EntityModels.JwtAuth;

namespace ProductManagement.JwtAuth;

public class AuthEndpoints
{
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