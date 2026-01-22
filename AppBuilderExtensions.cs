using Microsoft.EntityFrameworkCore;

namespace ProductManagement;

public static class AppBuilderDb
{
    public static void ConfigureDb (this WebApplicationBuilder builder)

    {
        builder.Services.AddDbContext<ProductManagementDb>(options =>
                options.UseSqlite("Data Source=./Database/account.db"),
            ServiceLifetime.Scoped); // Each request gets its own instance
    }

}