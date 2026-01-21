using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static ProductManagement.ProductManagementEndpoints;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using ProductManagement.Auth;
using static ProductManagement.Auth.ConfigAuthBuilder;
namespace ProductManagement;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.ConfigureDb();
        //builder.ConfigureAuth();
        JwtSettings jwtSettings = builder.ConfigureAuth();

        //builder.  this is to go with the conffig Aith 2 builder in addition to and make this program mode modular
            



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
            app.UseMigrationsEndPoint();
        else
            app.UseExceptionHandler("/Error");
        app.MapGet("/error", () => "test error");

        app.MapGet("/api/products/", SearchAll)
            .RequireAuthorization();

        app.MapGet("/api/products/{id}", SearchById)
            .WithName("GetAccountById")
            .RequireAuthorization();

        app.MapPost("/api/products/", RegisterNewProduct)
            .RequireAuthorization();

        app.MapDelete("/api/delete/{id}", DeleteById)
            .RequireAuthorization();


        // Authentication related - get new tokens  Login endpoint (public)
        app.MapPost("/auth/login", AuthEndpoints.GetValidToken)
            .WithName("Login")
            .WithTags("Authentication");


        // Authentication related - Token validation endpoint (protected)
        app.MapGet("/validate-token", AuthEndpoints.GetTokenValidity)
            .RequireAuthorization()
            .WithName("ValidateToken")
            .WithTags("Protected");

        app.Run();
    }
}