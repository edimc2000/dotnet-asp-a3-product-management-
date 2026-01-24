using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ProductManagement.EntityModels;

namespace ProductManagement.Helper
{
    /// <summary>Helper utilities and constants for Product Management operations.</summary>
    internal partial class Helper
    {
        /// <summary>Development configuration loaded from dev_config.json.</summary>
        public static IConfiguration DevConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("dev_config.json", true, true)
            .Build();


        /// <summary>Array of restricted account IDs that cannot be modified or deleted.</summary>
        internal static readonly int[] restrictedIds = { 101, 102, 103 };
        
        /// <summary>Converts JSON elements to account input data properties.</summary>
        internal class InputDataConverter
        {
      
            public JsonElement name { get; set; }

     
            public JsonElement description { get; set; }


            public JsonElement price { get; set; }
        }

        /// <summary>Returns successful product deletion response.</summary>
        /// <returns>An IResult containing the deletion success response.</returns>
        public static IResult DeleteSuccess()
        {
            return Results.Ok(
                new
                {
                    success = true,
                    message = "Product deleted successfully"
                });
        }


        /// <summary>Returns successful product search response.</summary>
        /// <param name="productList">The list of Product objects to return in the response.</param>
        /// <returns>An IResult containing the success response with product data.</returns>
        public static IResult SearchSuccess(List<Product> productList)
        {
            string message = $"{
                (productList.Count < 1 ? "There are no products on the database"
                    : productList.Count > 1
                        ? $"Total of {productList.Count} products retrieved successfully"
                        : "Product retrieved successfully")}";

            return Results.Ok(new
            {
                success = true,
                message = message,
                data = productList.ToList()
            });
        }

        
        /// <summary>Returns formatted bad request response.</summary>
        /// <param name="message">Error message.</param>
        /// <returns>An IResult containing the bad request error response.</returns>
        public static IResult BadRequest(string message)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = message
            });
        }

        /// <summary>Returns formatted forbidden response.</summary>
        /// <param name="message">Error message.</param>
        /// <returns>An IResult containing the forbidden error response.</returns>
        public static IResult Forbidden(string message)
        {
            return Results.Json(
                data: new { success = false, message = message },
                statusCode: StatusCodes.Status403Forbidden
            );
        }


        /// <summary>Returns formatted not found response.</summary>
        /// <param name="message">Error message.</param>
        /// <returns>An IResult containing the not found error response.</returns>
        public static IResult NotFound(string message)
        {
            return Results.NotFound(new
            {
                success = false,
                message = message
            });
        }

        /// <summary>Returns formatted unprocessable entity response.</summary>
        /// <param name="message">Error message.</param>
        /// <returns>An IResult containing the unprocessable entity error response.</returns>
        public static IResult UnprocessableEntity(string message)
        {
            return Results.UnprocessableEntity(new
            {
                success = false,
                message = $"{message}"
            });
        }

        /// <summary>Returns successful product creation response.</summary>
        /// <param name="newProduct">The newly created Product object.</param>
        /// <param name="newIdNumber">The ID of the newly created product.</param>
        /// <returns>An IResult containing the creation success response.</returns>
        public static IResult CreateSuccess(Product newProduct, int newIdNumber)
        {
            return Results.CreatedAtRoute("GetAccountById",
                new { id = newIdNumber },
                new
                {
                    success = true,
                    message = "Product created successfully",
                    data = newProduct
                });
        }

        /// <summary>Generates a new unique product ID number.</summary>
        /// <param name="db">The database context for product operations.</param>
        /// <returns>The next available product ID.</returns>
        public static int GetNewProductIdNumber( ProductManagementDb db)
        {
            return  db.Products
                .AsNoTracking()
                .Max(account => (account.ProductId)) + 1;      
        }
    }
}

