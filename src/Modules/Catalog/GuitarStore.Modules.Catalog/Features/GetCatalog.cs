using BuildingBlocks.Core.Caching;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.Catalog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Catalog.Features;

internal sealed record GetCatalogRequest(int? CategoryId = null, int PageSize = 10, int PageNumber = 1);

internal sealed class GetCatalog : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/catalog", async ([AsParameters] GetCatalogRequest request, CatalogDbContext dbContext, 
                CancellationToken ct) =>
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
                .Select(p => new ProductSummary
                (
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Category.Name
                )).ToListAsync(ct);

            var total = await catalogQuery.AsNoTracking().CountAsync(ct);

            var catalog = new GetCatalogResponse(products, total, request.PageNumber);

            return catalog.Products.Any() ? Results.Ok(catalog)
                                          : Results.Ok("Catalog is empty");
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetCatalog>>() 
        .AddEndpointFilter<CachingEndpointFilter<GetCatalogRequest, GetCatalogResponse>>()
        .WithName("GetCatalog")
        .WithTags("Catalog")
        .AllowAnonymous();

        return builder;
    }
}

internal sealed record GetCatalogResponse(
    ICollection<ProductSummary> Products,
    int TotalResults,
    int PageNumber);
internal sealed record ProductSummary(
    Guid Id,
    string Name,
    decimal Price,
    string Category);

