using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.Catalog.Errors;

internal sealed class ProductNotFoundError : NotFoundError
{
    public ProductNotFoundError(Guid productId) : base("Not Found", $"Item with id '{productId}' not found.")
    {}
}
