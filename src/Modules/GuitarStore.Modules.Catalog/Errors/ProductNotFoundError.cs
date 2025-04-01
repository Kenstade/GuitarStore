using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.Catalog.Errors;

public sealed class ProductNotFoundError : NotFoundError
{
    public ProductNotFoundError(string productId) : base("Not Found", $"Item with id '{productId}' not found.")
    {}
}
