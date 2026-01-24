namespace ProductManagement.AppBuilder;

/// <summary>Application builder extensions for Razor Pages configuration.</summary>
public static class AppBuilderRazor
{
    /// <summary>Configures Razor Pages services for the application.</summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    public static void ConfigureRazor (this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
    }

}