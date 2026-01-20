using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static ProductManagement.Helper.Helper;


namespace ProductManagement;

public class ProductManagementEndpoints
{
    /// <summary>Searches for and returns all accounts in the database.</summary>
    /// <param name="db">The database context for account operations.</param>
    /// <returns>An IResult containing the search results.</returns>
    public static async Task<IResult> SearchAll(ProductManagementDb db)
    {
        List<Product> productList = await db.Products.AsNoTracking().ToListAsync();
        return SearchSuccess(productList);
    }


    /// <summary>Searches for and returns all accounts in the database.</summary>
    /// <param name="db">The database context for account operations.</param>
    /// <returns>An IResult containing the search results.</returns>
    public static async Task<IResult> SearchById(string id, ProductManagementDb db)
    {
        if (!int.TryParse(id, out int parsedId))
            return BadRequest($"'{id}' is not a valid account Id");

        Product? product = await db.Products.FindAsync(parsedId);

        if (product == null)
            return NotFound($"Product with ID '{parsedId}' was not found.");

        List<Product> productList = new();
        productList.Add(product);


        //Product inputData = new()
        //{
        //    LastAccessedAt = DateTime.UtcNow,
        //    LastAccessedBy = "_search_api"
        //};
        // updating accessed by and accessed at 
        product.LastAccessedAt = DateTime.UtcNow;
        product.LastAccessedBy = "_search_api";
        await db.SaveChangesAsync();


        return SearchSuccess(productList);
    }


    public static async Task<IResult> RegisterNewProduct
        (HttpContext context, ProductManagementDb db)
    {
        (InputDataConverter? dataConverter, IResult? error) =
            await TryReadJsonBodyAsync<InputDataConverter>(context.Request);

        if (error != null)
            return error;


        if (!double.TryParse(dataConverter?.price.ToString(), out double parsedId))
            return BadRequest($"'{dataConverter?.price.ToString()}' is not a valid amount");

        Product inputData = new()
        {
            ProductId = GetNewProductIdNumber(db),
            Name = dataConverter? .name.ToString() ?? string.Empty,
            Description = dataConverter?.description.ToString()?? string.Empty,
            Price = parsedId, 
            CreatedAt = DateTime.UtcNow,
            LastAccessedBy = "_create_api"
        };


        ValidationContext validationContext = new(inputData);
        List<ValidationResult> validationResults = new();
        bool isValid = Validator.TryValidateObject(inputData,
            validationContext,
            validationResults,
            true);

        if (!isValid)
        {
            WriteLine(" -----> validation NOT ok ");
            string errors = string.Join(". ",
                validationResults.Select(r => r.ErrorMessage));
            return BadRequest($"Validation Error: {errors}.");
        }

        WriteLine(" -----> validation ok ");

        db.Add(inputData);
        await db.SaveChangesAsync();

        return Results.Ok("f");
    }
}