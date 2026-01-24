using ProductManagement.Helper;
using ProductManagement.AppBuilder;
using ProductManagement.JwtAuth;

namespace ProductManagement;

/// <summary>Main program class for the Product Management API.</summary>
/// <para>Author: Eddie C.</para>
/// <para>Version: 1.0</para>
/// <para>Date: Jan. 23, 2026</para>
public class Program
{
    /// <summary>Main entry point for the Product Management application.</summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // dev config and environment
        builder.Configuration.AddJsonFile("dev_config.json", true, true);

        //Add controllers 
        builder.ConfigureControllers();

        // Add services to the container - database
        builder.ConfigureDb();

        // Add services to the container - authentication / authorization
        JwtSettings jwtSettings = builder.ConfigureAuth();

        // Add razor pages - pages for apidoc and dev_tokens 
        builder.ConfigureRazor();


        WebApplication app = builder.Build();

        // Standard middleware in proper order
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.MapControllers();
        
        // This ensures your database is ready with migrations
        // Applied and optimized for SQLite before your app starts handling requests
        app.ApplyDatabaseMigrations();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.Run();
    }
}