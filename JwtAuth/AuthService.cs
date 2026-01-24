using static ProductManagement.Helper.Helper;

namespace ProductManagement.EntityModels.JwtAuth;

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

    public UserModel? Authenticate(string username, string password)
    {
        // Simple validation for testing (in production, use proper hashing!)
        //bool isValid = (username == "admin" && password == "admin123") ||
        //               (username == "user" && password == "user123");
        bool isValid = (username == DevConfiguration["Users:readWrite:userName"]
                        && password == DevConfiguration["Users:readWrite:password"]) ||
                       (username == DevConfiguration["Users:readOnly:userName"]
                        && password == DevConfiguration["Users:readOnly:password"]);

        if (!isValid)
            return null;

        return _users.FirstOrDefault(u => u.Username == username);
    }
}