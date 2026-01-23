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

        // env and config 
        builder.Configuration.AddJsonFile("dev_config.json", optional: true, reloadOnChange: true);
        
        //Add controllers 
        builder.ConfigureControllers();

        // Add services to the container - database
        builder.ConfigureDb();

        // Add services to the container - authentication / authorization
        JwtSettings jwtSettings = builder.ConfigureAuth(); 

        // Add razor pages 
        builder.ConfigureRazor();
        

        WebApplication app = builder.Build();
        
        // Standard middleware ordering:
        app.UseHttpsRedirection();
        app.UseStaticFiles();      // enable wwwroot static assets
        app.UseRouting();          // MUST come before authentication/authorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.MapControllers();


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
            app.UseExceptionHandler("/error");



        // used controllers for below 
        //app.MapGet("/error", () => "test error");

        //app.MapGet("/api/products/", SearchAll)
        //    .RequireAuthorization();

        //app.MapGet("/api/products/{id}", SearchById)
        //    .WithName("GetAccountById")
        //    .RequireAuthorization();

        //app.MapPost("/api/products/", RegisterNewProduct)
        //    .RequireAuthorization("ReadWrite");

        //app.MapDelete("/api/delete/{id}", DeleteById)
        //    .RequireAuthorization("ReadWrite");


        // Get new tokens  - Login endpoint (public)
        //app.MapPost("/auth/login", AuthEndpoints.GetValidToken)
        //    .WithName("Login")
        //    .WithTags("Authentication");


        ////Token validation endpoint (protected)
        //app.MapGet("/auth/validate-token", AuthEndpoints.GetTokenValidity)
        //    .RequireAuthorization()
        //    .WithName("ValidateToken")
        //    .WithTags("Protected");

        app.Run();
    }
}