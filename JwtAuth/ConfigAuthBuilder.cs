// API assistance was used on this class, both logic and documentation 

using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using ProductManagement.EntityModels.Data;


namespace ProductManagement.JwtAuth;

/// <summary>Application builder extensions for JWT authentication configuration.</summary>
public static class AppBuilderAuth
{
    /// <summary>Configures JWT authentication, authorization policies, and identity services.</summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    /// <returns>The configured JwtSettings instance.</returns>
    public static JwtSettings ConfigureAuth(this WebApplicationBuilder builder)
    {
        // Configure JWT Settings
        JwtSettings jwtSettings = new();
        builder.Services.AddSingleton(jwtSettings);

        // Register Identity DbContext and Identity services so UserManager/SignInManager are available
        // Use a separate SQLite file for identity tables (future feature), for storing/reading users to/from a db ==> future development
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=./Database/identity.db"));

        // Registers UserManager<IdentityUser>, SignInManager<IdentityUser>, RoleManager<IdentityRole>, etc.
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // Add Services for JWT token generation / auth logic
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

        // Authorization policies
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ReadOnly", policy =>
                policy.RequireRole("User", "Admin"));

            options.AddPolicy("ReadWrite", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("User", policy =>
                policy.RequireRole("User"));

            options.AddPolicy("Admin", policy =>
                policy.RequireRole("Admin"));
        });

        return jwtSettings;
    }
}