using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProductManagement.Models;

namespace ProductManagement;

public static class AppBuilderExtensions
{
    public static void ConfigureAuthenticationAndAuthorization
        (this WebApplicationBuilder builder)
    //(this WebApplicationBuilder builder, JwtSettings jwtSettings)
    {
        builder.Services.AddDbContext<ProductManagementDb>(options =>
                options.UseSqlite("Data Source=./Database/account.db"),
            ServiceLifetime.Scoped); // Each request gets its own instance


        // Configure JWT Settings
        var jwtSettings = new JwtSettings();
        
        builder.Services.AddSingleton(jwtSettings);

// Add Services
        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // Configure Authentication
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();


    }
}