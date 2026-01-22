using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static ProductManagement.ProductManagementEndpoints;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using ProductManagement.JwtAuth;
using ProductManagement.Helper;

namespace ProductManagement;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container - database
        builder.ConfigureDb();

        // Add services to the container - authentication / authorization
        JwtSettings jwtSettings = builder.ConfigureAuth(); 
        
        WebApplication app = builder.Build();
        
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();


        //This ensures your database is ready with migrations
        //applied and optimized for SQLite before your app starts handling requests
        app.ApplyDatabaseMigrations();
        

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseDeveloperExceptionPage();
        }
        else
            app.UseExceptionHandler("/Error");


        app.MapGet("/error", () => "test error");

        app.MapGet("/api/products/", SearchAll)
            .RequireAuthorization();

        app.MapGet("/api/products/{id}", SearchById)
            .WithName("GetAccountById")
            .RequireAuthorization();

        app.MapPost("/api/products/", RegisterNewProduct)
            .RequireAuthorization("ReadWrite");
            //.RequireAuthorization();
            //.RequireAuthorization(policy => policy.RequireRole("User", "Admin")); 

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