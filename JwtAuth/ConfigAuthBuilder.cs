using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProductManagement.JwtAuth;

namespace ProductManagement.JwtAuth;

public static class AppBuilderAuth
{
    public static JwtSettings ConfigureAuth
        ( this WebApplicationBuilder builder)
    {

        // Configure JWT Settings
        JwtSettings jwtSettings = new();
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
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        //builder.Services.AddAuthorization();
        // granular readonly and read/write privilege 

        builder.Services.AddAuthorization(options =>
        {
            // Policy for read-only access (Users)
            options.AddPolicy("ReadOnly", policy =>
                policy.RequireRole("User", "Admin")); // Both users and admins can read
            
            // Policy for read-write access (Admins only)
            options.AddPolicy("ReadWrite", policy =>
                policy.RequireRole("Admin"));
            
            // Or you can have separate policies
            options.AddPolicy("User", policy =>
                policy.RequireRole("User"));
                
            options.AddPolicy("Admin", policy =>
                policy.RequireRole("Admin"));
        });


        return jwtSettings;
    }
}