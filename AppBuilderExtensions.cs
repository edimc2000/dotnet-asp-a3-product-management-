using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProductManagement;

public static class AppBuilderExtensions
{
    public static void ConfigureAuthenticationAndAuthorization
        (this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ProductManagementDb>(options =>
                options.UseSqlite("Data Source=./Database/account.db"),
            ServiceLifetime.Scoped); // Each request gets its own instance

        builder.Services.AddAuthentication()
            .AddJwtBearer();
        builder.Services.AddAuthorization();




    }
}