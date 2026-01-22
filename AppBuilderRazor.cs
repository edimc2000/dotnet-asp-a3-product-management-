using Microsoft.EntityFrameworkCore;

namespace ProductManagement;

public static class AppBuilderRazor
{
    public static void ConfigureRazor (this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
    }

}