using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductManagement.EntityModels;
using ProductManagement.JwtAuth;



namespace ProductManagement;

/// <summary>API controller for product management operations.</summary>
/// <para>Author: Eddie C.</para>
/// <para>Version: 1.0</para>
/// <para>Date: Jan. 23, 2026</para>
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductManagementDb _db;

    /// <summary>Initializes a new instance of the ProductsController.</summary>
    /// <param name="db">The database context for product operations.</param>
    public ProductsController(ProductManagementDb db)
    {
        _db = db;
    }

    /// <summary>Retrieves all products from the database.</summary>
    /// <returns>An IResult containing all products.</returns>
    [HttpGet]
    [Authorize]
    public async Task<IResult> SearchAll()
    {
        return await ProductManagementEndpoints.SearchAll(_db);
    }

    /// <summary>Retrieves a product by its ID.</summary>
    /// <param name="id">The product ID to search for.</param>
    /// <returns>An IResult containing the product or error.</returns>
    [HttpGet("{id}", Name = "GetAccountById")]
    [Authorize]
    public async Task<IResult> SearchById(string id)
    {
        return await ProductManagementEndpoints.SearchById(id, _db);
    }

    /// <summary>Registers a new product in the database.</summary>
    /// <returns>An IResult indicating success or validation errors.</returns>
    [HttpPost]
    [Authorize(Policy = "ReadWrite")]
    public async Task<IResult> RegisterNewProduct()
    {
        return await ProductManagementEndpoints.RegisterNewProduct(HttpContext, _db);
    }

    /// <summary>Deletes a product by its ID.</summary>
    /// <param name="id">The product ID to delete.</param>
    /// <returns>An IResult indicating success or error.</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "ReadWrite")]
    public async Task<IResult> DeleteById(string id)
    {
        return await ProductManagementEndpoints.DeleteById(id, _db);
    }
}

// extras for testing and development purposes 
/// <summary>Authentication controller for token management.</summary>
[ApiController]
[Route("auth")] // Direct route mapping
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;

    /// <summary>Initializes a new instance of the AuthController.</summary>
    /// <param name="tokenService">Service for JWT token operations.</param>
    /// <param name="authService">Service for authentication operations.</param>
    /// <param name="jwtSettings">Configuration settings for JWT.</param>
    public AuthController
    (
        ITokenService tokenService,
        IAuthService authService,
        IOptions<JwtSettings> jwtSettings
    )
    {
        _tokenService = tokenService;
        _authService = authService;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>Generates a valid JWT token for authenticated users.</summary>
    /// <param name="request">The login request containing credentials.</param>
    /// <returns>An IResult containing the token or authentication error.</returns>
    [HttpPost("login")]
    public async Task<IResult> GetValidToken(LoginRequest request)
    {
        WriteLine($"--- get token controller test ");
        return AuthEndpoints.GetValidToken(request, _tokenService, _authService, _jwtSettings);
    }

    /// <summary>Validates the current user's JWT token.</summary>
    /// <returns>An IResult indicating token validity.</returns>
    [HttpGet("validate-token")]
    [Authorize]
    public async Task<IResult> GetTokenValidity()
    {
        WriteLine($"--- get token validity controller test ");
        return AuthEndpoints.GetTokenValidity(HttpContext);
    }
}

/// <summary>Error handling controller for displaying error messages.</summary>
[ApiController]
[Route("error")] // Direct route mapping
public class ErrorController : ControllerBase
{
    /// <summary>Displays a generic error message.</summary>
    /// <returns>An IActionResult with error information.</returns>
    public IActionResult DisplayError()
    {
        return Ok("There is an error on the page");
    }
}