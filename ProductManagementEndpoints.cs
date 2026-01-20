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

        Product ? product = await db.Products.FindAsync(parsedId);

        if (product == null)
            return NotFound($"Product with ID '{parsedId}' was not found.");

        List<Product> productList = new List<Product>();
        productList.Add(product);
        
        return SearchSuccess(productList);
    }
}