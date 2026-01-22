namespace ProductManagement.JwtAuth
{

        public record LoginRequest(string Username, string Password);
        public record LoginResponse(string Token, string Message);

        public class UserModel
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }

        public class JwtSettings
        {
            public string Key { get; set; } = "super-secret-key-minimum-32-characters-long-here";
            public string Issuer { get; set; } = "minimal-api";
            public string Audience { get; set; } = "api-users";
            public int ExpiryInMinutes { get; set; } = 120;
        }
 
        public interface ITokenService
        {
            string GenerateToken(UserModel user);
        }

}
