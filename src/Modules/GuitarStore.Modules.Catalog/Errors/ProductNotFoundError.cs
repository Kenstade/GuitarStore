using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.Catalog.Errors;

public class ProductNotFoundError : NotFoundError
{
    public ProductNotFoundError(int productId) : base("Not Found", $"Item with id '{productId}' not found")
    {}
}
