using BuildingBlocks.Core.ErrorHandling;

namespace GuitarStore.Modules.Catalog.Contracts;

public interface ICatalogService
{
    Task<Result<ProductDetails>> GetProductForCartAsync(Guid productId, CancellationToken ct = default);
}

public sealed record ProductDetails(string Name, string? Image, decimal Price);