using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using static ProductManagement.ProductManagementEndpoints;

namespace ProductManagement;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.ConfigureAuthenticationAndAuthorization();


        WebApplication app = builder.Build();

        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();

        using (IServiceScope scope = app.Services.CreateScope())
        {
            ProductManagementDb
                db = scope.ServiceProvider.GetRequiredService<ProductManagementDb>();
            db.Database.Migrate(); // Apply any pending migrations

            db.Database.ExecuteSqlRaw("PRAGMA journal_mode = WAL;"); // Enable WAL
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }
        app.MapGet("/error", () => "test error");

        app.MapGet("/api/products/", SearchAll)
            .RequireAuthorization();

        app.MapGet("/api/products/{id}",  SearchById)
            .WithName("GetAccountById")
            .RequireAuthorization();

        app.MapPost("/api/products/",  RegisterNewProduct)
            .RequireAuthorization();
        
        app.MapDelete("/api/delete/{id}",  DeleteById)
            .RequireAuthorization();

    app.Run();
    }
}