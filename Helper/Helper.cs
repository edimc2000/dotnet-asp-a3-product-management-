using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ProductManagement.Helper
{
    internal partial class Helper
    {



        /// <summary>Array of restricted account IDs that cannot be modified or deleted.</summary>
        internal static readonly int[] restrictedIds = { 101, 102, 103 };



        /// <summary>Converts JSON elements to account input data properties.</summary>
        internal class InputDataConverter
        {
      
            public JsonElement name { get; set; }

     
            public JsonElement description { get; set; }


            public JsonElement price { get; set; }
        }


        public static IResult DeleteSuccess()
        {
            return Results.Ok(
                new
                {
                    success = true,
                    message = "Product deleted successfully"
                });
        }


        //internal class DataValidation
        //{
        //    [Required]
        //    [StringLength(100, MinimumLength = 2, ErrorMessage = "Description must be at least 2 characters long")]
        //    public string name { get; set; }

        //    [Required]
        //    [StringLength(100, MinimumLength = 2, ErrorMessage = "Description must be at least 2 characters long")]
        //    public string description { get; set; }

        //    [Required]
        //    [Range(1.0, 999999.0, ErrorMessage = "Price must be between 1 and 999,999")]
        //    public double price { get; set; }
        //}

        /// <summary>
        /// Returns successful all accounts response.</summary>
        /// <param name="accountList">The list of Account objects to return in the response.</param>
        /// <returns>An IResult containing the success response with account data.</returns>
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


        public static IResult Forbidden(string message)
        {
            return Results.Json(
                data: new { success = false, message = message },
                statusCode: StatusCodes.Status403Forbidden
            );
        }


        public static IResult NotFound(string message)
        {
            return Results.NotFound(new
            {
                success = false,
                message = message
            });
        }


        public static IResult UnprocessableEntity(string message)
        {
            return Results.UnprocessableEntity(new
            {
                success = false,
                message = $"{message}"
            });
        }



        public static void timeUpdate( ProductManagementDb db, string id)
        {

        }

        public static int GetNewProductIdNumber( ProductManagementDb db)
        {
            return  db.Products
                .AsNoTracking()
                .Max(account => (account.ProductId)) + 1;      
        }
    }
}



// ***** below is to read the key:value from body - json content 
//using StreamReader reader = new(context.Request.Body);
//string jsonString = await reader.ReadToEndAsync();

//using JsonDocument jsonDoc = JsonDocument.Parse(jsonString);
//JsonElement root = jsonDoc.RootElement;

//// Get keys from JSON object
//List<string> keys = root.EnumerateObject()
//    .Select(p => p.Name)
//    .ToList();

//List<JsonElement> values = root.EnumerateObject()
//    .Select(p => p.Value)
//    .ToList();


//WriteLine($" ****************** keys: {string.Join(", ", keys)} ");
//WriteLine($" ****************** values: {string.Join(", ", values)} ");