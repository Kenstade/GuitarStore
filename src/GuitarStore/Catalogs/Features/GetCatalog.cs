using GuitarStore.Common.Caching;
using GuitarStore.Common.Web;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Catalogs.Features;

public sealed record GetCatalogRequest(int? CategoryId = null, int PageSize = 10, int PageNumber = 1) 
    : ICacheRequest
{
    public string CacheKey => "GetCatalog";
}
internal sealed class GetCatalog : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("", HandleAsync)
        .AllowAnonymous();
    private static async Task<IResult> HandleAsync([AsParameters]GetCatalogRequest request, 
        AppDbContext dbContext, ICacheProvider cache)
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
                    p.Image,
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
    string Image, 
    decimal Price, 
    string Category);
