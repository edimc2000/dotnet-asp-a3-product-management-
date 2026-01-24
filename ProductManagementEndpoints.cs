using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static ProductManagement.EntityModels.Helper.Helper;

namespace ProductManagement.EntityModels;

public class ProductManagementEndpoints
{
    /// <summary>Searches for and returns all products in the database.</summary>
    /// <param name="db">The database context for product operations.</param>
    /// <returns>An IResult containing the search results.</returns>
    public static async Task<IResult> SearchAll(ProductManagementDb db)
    {
        List<Product> productList = await db.Products.AsNoTracking().ToListAsync();
        return SearchSuccess(productList);
    }
    
    /// <summary>Searches for and returns a product by its ID.</summary>
    /// <param name="id">The product ID to search for.</param>
    /// <param name="db">The database context for product operations.</param>
    /// <returns>An IResult containing the search result or error.</returns>
    public static async Task<IResult> SearchById(string id, ProductManagementDb db)
    {
        if (!int.TryParse(id, out int parsedId))
            return BadRequest($"'{id}' is not a valid ProductId");

        Product? product = await db.Products.FindAsync(parsedId);

        if (product == null)
            return NotFound($"ProductId '{parsedId}' was not found.");

        List<Product> productList = new();
        productList.Add(product);
        
        product.LastAccessedAt = DateTime.UtcNow;
        product.LastAccessedBy = "_search_api";
        await db.SaveChangesAsync();
        return SearchSuccess(productList);
    }

    /// <summary>Registers a new product in the database.</summary>
    /// <param name="context">The HTTP context containing the request data.</param>
    /// <param name="db">The database context for product operations.</param>
    /// <returns>An IResult indicating success or validation errors.</returns>
    public static async Task<IResult> RegisterNewProduct
        (HttpContext context, ProductManagementDb db)
    {
        (Helper.Helper.InputDataConverter? dataConverter, IResult? error) =
            await TryReadJsonBodyAsync<Helper.Helper.InputDataConverter>(context.Request);

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
            CreatedBy = "_create_api"
        };

        ValidationContext validationContext = new(inputData);
        List<ValidationResult> validationResults = new();
        bool isValid = Validator.TryValidateObject(inputData,
            validationContext,
            validationResults,
            true);

        if (!isValid)
        {
            string errors = string.Join("; ",
                validationResults.Select(r => r.ErrorMessage));
            return BadRequest($"Validation Error: {errors}");
        }
        
        db.Add(inputData);
        await db.SaveChangesAsync();
        return CreateSuccess(inputData, inputData.ProductId);
    }

    /// <summary>Deletes a product by its ID.</summary>
    /// <param name="id">The product ID to delete.</param>
    /// <param name="db">The database context for product operations.</param>
    /// <returns>An IResult indicating success or error.</returns>
    public static async Task<IResult> DeleteById
        (string id, ProductManagementDb db)
    {
        if (!int.TryParse(id, out int parsedId))
            return BadRequest($"'{id}' is not a valid ProductId");

        Product? account = await db.Products.FindAsync(parsedId);

        if (account == null)
            return NotFound($"'{id}' is not a valid ProductId");

        if (restrictedIds.Contains(parsedId))
            return Forbidden($"ProductId '{id}' is restricted and cannot be deleted");

        db.Products.Remove(account);
        await db.SaveChangesAsync();
        return DeleteSuccess();
    }

}