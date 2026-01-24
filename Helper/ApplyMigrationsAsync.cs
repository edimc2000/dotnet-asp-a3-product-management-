using Microsoft.EntityFrameworkCore;
using ProductManagement.EntityModels;

namespace ProductManagement.Helper;

/// <summary>Database migration utilities for Product Management.</summary>
public static class DbMigration
{
    /// <summary>Applies pending database migrations and optimizes SQLite with WAL mode.</summary>
    /// <param name="app">The WebApplication to apply migrations for.</param>
    public static void ApplyDatabaseMigrations(this WebApplication app)
    {
        using (IServiceScope scope = app.Services.CreateScope())
        {
            ProductManagementDb
                db = scope.ServiceProvider.GetRequiredService<ProductManagementDb>();
            db.Database.Migrate();
            db.Database.ExecuteSqlRaw("PRAGMA journal_mode = WAL;");
        }
    }
}