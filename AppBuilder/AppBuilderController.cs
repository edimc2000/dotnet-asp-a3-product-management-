using Microsoft.EntityFrameworkCore;

namespace ProductManagement.AppBuilder;

public static class AppBuilderController
{
    public static void ConfigureControllers (this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
    }

}