using GuitarStore.Common.Web.Errors;

namespace GuitarStore.Catalogs.Errors;

public class ProductNotFoundError : NotFoundError
{
    public ProductNotFoundError(int productId) : base("Not Found", $"Item with id '{productId}' not found")
    {}
}
