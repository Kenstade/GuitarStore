using BuildingBlocks.Core.Caching;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.Catalog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Catalog.Features;

public sealed record GetCatalogRequest(int? CategoryId = null, int PageSize = 10, int PageNumber = 1);

internal sealed class GetCatalog : IEndpoint
{
    private readonly CatalogDbContext _dbContext;
    public GetCatalog(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/catalog", async ([AsParameters] GetCatalogRequest request, CancellationToken ct) =>
        {
            return await Handle(request, ct);
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetCatalogRequest>>() 
        .AddEndpointFilter<CachingEndpointFilter<GetCatalogRequest, GetCatalogResponse>>()
        .WithName("GetCatalog")
        .WithTags("Catalog")
        .AllowAnonymous();

        return builder;
    }

    private async Task<IResult> Handle(GetCatalogRequest request, CancellationToken ct)
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
                p.Id,
                p.Name,
                p.Price,
                p.Category.Name
            )).ToListAsync(ct);

        var total = await catalogQuery.AsNoTracking().CountAsync(ct);

        var catalog = new GetCatalogResponse(products, total, request.PageNumber);

        return catalog.Products.Any() ? TypedResults.Ok(catalog)
                                      : TypedResults.Ok("Catalog is empty");
    }
}

public sealed record GetCatalogResponse(
    ICollection<ProductPartialResponse> Products,
    int TotalResults,
    int PageNumber);
public sealed record ProductPartialResponse(
    Guid Id,
    string Name,
    decimal Price,
    string Category);

