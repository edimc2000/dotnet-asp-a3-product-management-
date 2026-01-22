using Microsoft.EntityFrameworkCore;
namespace ProductManagement.Helper;

public static class DbMigration
{
    public static void ApplyDatabaseMigrations(this WebApplication app)
    {
        using (IServiceScope scope = app.Services.CreateScope())
        {
            ProductManagementDb db = scope.ServiceProvider.GetRequiredService<ProductManagementDb>();
            db.Database.Migrate();
            db.Database.ExecuteSqlRaw("PRAGMA journal_mode = WAL;");
        }
    }
}