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
            // The default HSTS value is 30 days. You may want to change this for production
            // scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }


        app.MapGet("/error", () => "test error");

        app.MapGet("/api/products/", SearchAll);
        app.MapGet("/api/products/{id}",  SearchById);

        app.MapPost("/api/products/",  RegisterNewProduct);

    app.Run();
    }
}