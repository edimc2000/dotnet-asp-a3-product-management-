using Microsoft.EntityFrameworkCore;
namespace ProductManagement.EntityModels.AppBuilder;

/// <summary>Application builder extensions for database configuration.</summary>
public static class AppBuilderDb
{
    /// <summary>Configures the SQLite database context for the application.</summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    public static void ConfigureDb(this WebApplicationBuilder builder)
    {
        string ConnectionString = builder.Configuration
            .GetSection("Database")["Details:ConnectionString"];

        // Each request gets its own instance
        builder.Services.AddDbContext<ProductManagementDb>(options =>
                options.UseSqlite(ConnectionString),
            ServiceLifetime.Scoped); 
    }
}