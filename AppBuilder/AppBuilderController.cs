namespace ProductManagement.AppBuilder;

/// <summary>Application builder extensions for controller configuration.</summary>
public static class AppBuilderController
{
    /// <summary>Configures ASP.NET Core controllers for the application.</summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    public static void ConfigureControllers (this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
    }

}