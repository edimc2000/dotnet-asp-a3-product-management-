using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductManagement.JwtAuth;

namespace ProductManagement
{
    [ApiController]
    [Route("api/products")]  // Direct route mapping
    public class ProductsController : ControllerBase
    {
        private readonly ProductManagementDb _db;

        public ProductsController(ProductManagementDb db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize]
        public async Task<IResult> SearchAll()
        {
            WriteLine($"Route accessed: {HttpContext.Request.Path}");
            return await ProductManagementEndpoints.SearchAll(_db);
        }

        // You can add other product endpoints
        [HttpGet("{id}", Name = "GetAccountById")]
        [Authorize]
        public async Task<IResult> SearchById(string id)
        {
            WriteLine($"Route accessed: {HttpContext.Request.Path}");
            WriteLine($"--- this is on the controller {id}");
            return await ProductManagementEndpoints.SearchById(id, _db);
        }


        // You can add other product endpoints
        [HttpPost("")]
        [Authorize(Policy = "ReadWrite")]
        public async Task<IResult> RegisterNewProduct()
        {
            WriteLine($"--- this is on the controller register");
            return await ProductManagementEndpoints.RegisterNewProduct(HttpContext, _db);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ReadWrite")]
        public async Task<IResult> DeleteById(string id)
        {
            WriteLine($"--- this is on the controller delete");
            return await ProductManagementEndpoints.DeleteById(id, _db);
        }
    }


    // extras for testing and development purposes 
    [ApiController]
    [Route("auth")] // Direct route mapping
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(
            ITokenService tokenService,
            IAuthService authService,
            IOptions<JwtSettings> jwtSettings)
        {
            _tokenService = tokenService;
            _authService = authService;
            _jwtSettings = jwtSettings.Value;  // Note: .Value is used here
        }

        [HttpPost("login")]
        public async Task<IResult> GetValidToken(LoginRequest request)
        {
            WriteLine($"--- get token controller test ");
            return AuthEndpoints.GetValidToken(request, _tokenService, _authService, _jwtSettings);
        }
     

        [HttpGet("validate-token")]
        [Authorize]
        public async Task<IResult> GetTokenValidity()
        {
            WriteLine($"--- get token validity controller test ");
            return AuthEndpoints.GetTokenValidity(HttpContext);
        }
    }



    [ApiController]
    [Route("error")] // Direct route mapping
    public class ErrorController : ControllerBase
    {
        public IActionResult  DisplayError()
        {
            return Ok("There is an error on the page");
        }
    }
    

}


