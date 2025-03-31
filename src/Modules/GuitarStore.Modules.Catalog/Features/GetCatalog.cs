using BuildingBlocks.Core.Caching;
using BuildingBlocks.Web.EndpointFilters;
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
public sealed class GetCatalog(CatalogDbContext dbContext, ICacheProvider cache) : IEndpoint
{
    private readonly CatalogDbContext _dbContext = dbContext;
    private readonly ICacheProvider _cache = cache; 
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/catalog", async ([AsParameters] GetCatalogRequest request, CancellationToken ct) =>
        {
            return await Handle(request, ct);
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetCatalogRequest>>()    
        .WithName("GetCatalog");

        return builder;
    }

    private async Task<IResult> Handle(GetCatalogRequest request, CancellationToken ct)
    {
        var catalog = await _cache.GetOrCreateAsync(request, async () =>
        {
            var catalogQuery = _dbContext.Products
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
                )).ToListAsync(ct);

            var total = await catalogQuery.AsNoTracking().CountAsync(ct);

            return new GetCatalogResponse(products, total, request.PageNumber);
        });

        return catalog.Products.Any() ? TypedResults.Ok(catalog)
                                      : TypedResults.Ok("Catalog is empty");
    }
}

public sealed record GetCatalogResponse(
    ICollection<ProductPartialResponse> Products,
    int TotalResults,
    int PageNumber);
public sealed record ProductPartialResponse(
    string Name,
    decimal Price,
    string Category);

