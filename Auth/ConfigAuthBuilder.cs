using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ProductManagement.Auth
{
    public static  class ConfigAuthBuilder
    {
        public static JwtSettings ConfigureAuth2()
        {
            JwtSettings jwtSettings = new();
            return jwtSettings;
        }

    }
}
