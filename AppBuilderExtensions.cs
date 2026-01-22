using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProductManagement.Auth;

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