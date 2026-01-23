using Microsoft.EntityFrameworkCore;

namespace ProductManagement.AppBuilder;

public static class AppBuilderDb
{
    public static void ConfigureDb(this WebApplicationBuilder builder)
    {
        string ConnectionString = builder.Configuration
            .GetSection("Database")["Details:ConnectionString"];

        builder.Services.AddDbContext<ProductManagementDb>(options =>
                options.UseSqlite(ConnectionString),
            ServiceLifetime.Scoped); // Each request gets its own instance
    }
}