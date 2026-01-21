using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static ProductManagement.ProductManagementEndpoints;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using ProductManagement.Models;
namespace ProductManagement;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.ConfigureAuthenticationAndAuthorization();


        WebApplication app = builder.Build();


        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();

        using (IServiceScope scope = app.Services.CreateScope())
        {
            ProductManagementDb
                db = scope.ServiceProvider.GetRequiredService<ProductManagementDb>();
            db.Database.Migrate(); // Apply any pending migrations

            db.Database.ExecuteSqlRaw("PRAGMA journal_mode = WAL;"); // Enable WAL
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
            app.UseMigrationsEndPoint();
        else
            app.UseExceptionHandler("/Error");
        app.MapGet("/error", () => "test error");

        app.MapGet("/api/products/", SearchAll)
            .RequireAuthorization();

        app.MapGet("/api/products/{id}", SearchById)
            .WithName("GetAccountById")
            .RequireAuthorization();

        app.MapPost("/api/products/", RegisterNewProduct)
            .RequireAuthorization();

        app.MapDelete("/api/delete/{id}", DeleteById)
            .RequireAuthorization();


        // Protected endpoint example

// 1. Welcome endpoint (public)
        app.MapGet("/", () => "Welcome to Minimal API with JWT Authentication!")
            .WithName("Home")
            .WithTags("Public");

// 2. Login endpoint (public)
        app.MapPost("/auth/login",
                (Models.LoginRequest request, ITokenService tokenService, IAuthService authService) =>
                {
                    if (string.IsNullOrEmpty(request.Username) ||
                        string.IsNullOrEmpty(request.Password))
                        return Results.BadRequest("Username and password are required");

                    UserModel? user = authService.Authenticate(request.Username, request.Password);

                    if (user == null) return Results.Unauthorized();

                    string token = tokenService.GenerateToken(user);

                    return Results.Ok(new LoginResponse(
                        token,
                        ////$"Welcome {user.Username}! Token valid for {jwtSettings.ExpiryInMinutes} minutes."
                        //$"Welcome {user.Username}! " +
                        //$"Token valid for {ExpiryInMinutes} minutes."
                    $"Welcome {user.Username}! Token valid for  minutes."
                    ));
                })
            .WithName("Login")
            .WithTags("Authentication");

// 3. Public endpoint (no auth required)
        app.MapGet("/public",
                () =>
                {
                    return Results.Ok(new
                    {
                        Message = "This is a public endpoint - no authentication required",
                        Timestamp = DateTime.UtcNow,
                        Status = "OK"
                    });
                })
            .WithName("PublicInfo")
            .WithTags("Public");

// 4. Protected endpoint (requires any authenticated user)
        app.MapGet("/secure",
                (HttpContext context) =>
                {
                    string userName = context.User.Identity?.Name ?? "Unknown";
                    string userId = context.User.FindFirst("userId")?.Value ?? "Unknown";
                    string role = context.User.FindFirst(ClaimTypes.Role)?.Value ?? "No Role";

                    return Results.Ok(new
                    {
                        Message = "You accessed a protected endpoint!",
                        User = new
                        {
                            Id = userId,
                            Name = userName,
                            Role = role
                        },
                        Timestamp = DateTime.UtcNow,
                        IsAuthenticated = context.User.Identity?.IsAuthenticated ?? false
                    });
                })
            .RequireAuthorization()
            .WithName("SecureData")
            .WithTags("Protected");

// 5. Admin-only endpoint (requires Admin role)
        app.MapGet("/admin",
                (HttpContext context) =>
                {
                    string userName = context.User.Identity?.Name ?? "Admin";

                    return Results.Ok(new
                    {
                        Message = $"Welcome to Admin Dashboard, {userName}!",
                        AdminData = new
                        {
                            ServerStatus = "Operational",
                            ActiveUsers = 42,
                            Uptime = "99.9%",
                            LastMaintenance = DateTime.UtcNow.AddDays(-1)
                        },
                        Timestamp = DateTime.UtcNow
                    });
                })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("AdminPanel")
            .WithTags("Admin");

// 6. User profile endpoint (shows all claims)
        app.MapGet("/profile",
                (ClaimsPrincipal user) =>
                {
                    var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();

                    return Results.Ok(new
                    {
                        UserName = user.Identity?.Name,
                        IsAuthenticated = user.Identity?.IsAuthenticated ?? false,
                        AuthenticationType = user.Identity?.AuthenticationType,
                        Claims = claims,
                        TotalClaims = claims.Count
                    });
                })
            .RequireAuthorization()
            .WithName("UserProfile")
            .WithTags("Protected");

// 7. Health check endpoint (public)
        app.MapGet("/health",
                () =>
                {
                    return Results.Ok(new
                    {
                        Status = "Healthy",
                        Timestamp = DateTime.UtcNow,
                        Service = "Minimal API JWT Authentication",
                        Version = "1.0.0"
                    });
                })
            .WithName("HealthCheck")
            .WithTags("Public");

// 8. Token validation endpoint (protected)
        app.MapGet("/validate-token",
                (HttpContext context) =>
                {
                    string? authHeader = context.Request.Headers.Authorization.FirstOrDefault();

                    var ValidUntil = context.User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

                    int.TryParse(ValidUntil, out int parsedId);
                    DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(parsedId).UtcDateTime;

                    return Results.Ok(new
                    {
                        Message = "Token is valid!",
                        TokenPresent = !string.IsNullOrEmpty(authHeader),
                        User = context.User.Identity?.Name,
                        Role = context.User.FindFirst(ClaimTypes.Role)?.Value,
                        //ValidUntil = context.User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value
                        ValidUntil = dateTime
                    });
                })
            .RequireAuthorization()
            .WithName("ValidateToken")
            .WithTags("Protected");

        app.Run();
    }
}