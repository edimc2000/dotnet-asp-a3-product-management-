namespace ProductManagement.Helper
{
    public class Helper
    {

        /// <summary>Returns successful all accounts response.</summary>
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


        public static IResult NotFound(string message)
        {
            return Results.NotFound(new
            {
                success = false,
                message = message
            });
        }
    }
}
