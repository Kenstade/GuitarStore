namespace GuitarStore.Modules.Catalog.Contracts;

public interface ICatalogService
{
    Task<ProductDetails?> GetProductForCartAsync(Guid productId, CancellationToken ct = default);
}

public sealed record ProductDetails(string Name, string? Image, decimal Price);