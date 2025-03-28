using BuildingBlocks.Core.Caching;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.Catalog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Catalog.Features;

public sealed record GetCatalogRequest(int? CategoryId = null, int PageSize = 10, int PageNumber = 1)
    : ICacheRequest
{
    public string CacheKey => "GetCatalog";
}
public sealed class GetCatalog : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/catalog",
            async ([AsParameters] GetCatalogRequest request, CatalogDbContext dbContext, ICacheProvider cache) =>
                await Handle(request, dbContext, cache))
            .WithName("GetCatalog");

        return builder;
    }

    private async Task<IResult> Handle(GetCatalogRequest request, CatalogDbContext dbContext, ICacheProvider cache)
    {
        var catalog = await cache.GetOrCreateAsync(request, async () =>
        {
            var catalogQuery = dbContext.Products
                .Include(p => p.Category)
                .Where(p => p.IsAvailable && (request.CategoryId == null || p.CategoryId == request.CategoryId))
                .AsQueryable();

            var products = await catalogQuery
                .AsNoTrackingWithIdentityResolution()
                .OrderBy(p => p.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductPartialResponse
                (
                    p.Name,
                    p.Price,
                    p.Category.Name
                )).ToListAsync();

            var total = await catalogQuery.AsNoTracking().CountAsync();

            return new GetCatalogResponse(products, total, request.PageNumber, request.PageSize);
        });

        return catalog != null ? TypedResults.Ok(catalog)
                               : TypedResults.Ok("Catalog is empty");
    }
}

public sealed record GetCatalogResponse(
    ICollection<ProductPartialResponse> Products,
    int TotalResults,
    int PageNumber,
    int PageSize);
public sealed record ProductPartialResponse(
    string Name,
    decimal Price,
    string Category);

