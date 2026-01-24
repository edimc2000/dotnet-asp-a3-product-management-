// API assistance was used on this class, both logic and documentation 

using static ProductManagement.Helper.Helper;

namespace ProductManagement.JwtAuth;

/// <summary>Interface for authentication services.</summary>
public interface IAuthService
{
    UserModel? Authenticate(string username, string password);
}

public class AuthService : IAuthService
{
    // In-memory user store for testing
    private readonly List<UserModel> _users = new()
    {
        new UserModel { Id = 1, Username = "admin", Role = "Admin" },
        new UserModel { Id = 2, Username = "user", Role = "User" }
    };

    /// <summary>Authenticates a user with username and password against configuration.</summary>
    /// <param name="username">The username to authenticate.</param>
    /// <param name="password">The password to authenticate.</param>
    /// <returns>The authenticated UserModel or null if authentication fails.</returns>
    public UserModel? Authenticate(string username, string password)
    {
        // Simple validation for testing (in production, use proper hashing!)
        bool isValid = (username == DevConfiguration["Users:readWrite:userName"]
                        && password == DevConfiguration["Users:readWrite:password"]) ||
                       (username == DevConfiguration["Users:readOnly:userName"]
                        && password == DevConfiguration["Users:readOnly:password"]);

        if (!isValid)
            return null;

        return _users.FirstOrDefault(u => u.Username == username);
    }
}