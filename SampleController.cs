using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductManagement
{
    [ApiController]
    [Route("api")]  // Direct route mapping
    public class ProductsController : ControllerBase
    {
        private readonly ProductManagementDb _db;

        public ProductsController(ProductManagementDb db)
        {
            _db = db;
        }

        [HttpGet("products")]
        [Authorize]
        public async Task<IResult> SearchAll()
        {
            WriteLine($"Route accessed: {HttpContext.Request.Path}");
            return await ProductManagementEndpoints.SearchAll(_db);
        }

        // You can add other product endpoints
        [HttpGet("products/{id}", Name = "GetAccountById")]
        [Authorize]
        public async Task<IResult> SearchById(string id)
        {
            WriteLine($"Route accessed: {HttpContext.Request.Path}");
            WriteLine($"--- this is on the controller {id}");
            return await ProductManagementEndpoints.SearchById(id, _db);
        }


        // You can add other product endpoints
        [HttpPost("products")]
        [Authorize(Policy = "ReadWrite")]
        public async Task<IResult> RegisterNewProduct()
        {
            WriteLine($"--- this is on the controller register");
            return await ProductManagementEndpoints.RegisterNewProduct(HttpContext, _db);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "ReadWrite")]
        public async Task<IResult> DeleteById(string id)
        {
            WriteLine($"--- this is on the controller delete");
            return await ProductManagementEndpoints.DeleteById(id, _db);
        }

    }



}


