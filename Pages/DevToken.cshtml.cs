using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProductManagement.EntityModels.Pages
{
    public class DevTokenModel : PageModel
    {
        // Static sample tokens shown in rows 1 and 3.
        // Replace with real samples if you have them.
        public string AdminStaticToken { get; private set; } =
            "ADMIN_STATIC_TOKEN_SAMPLE_eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."; 

        public string UserStaticToken { get; private set; } =
            "USER_STATIC_TOKEN_SAMPLE_eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

        public void OnGet()
        {
        }
    }
}