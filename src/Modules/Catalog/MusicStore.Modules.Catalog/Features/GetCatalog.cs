using BuildingBlocks.Core.Dapper;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Web.MinimalApi;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MusicStore.Modules.Catalog.Features;

internal sealed record GetCatalogRequest(int? CategoryId = null, int PageSize = 10, int PageNumber = 1);

internal sealed class GetCatalog : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/catalog", async ([AsParameters] GetCatalogRequest request, 
                IDbConnectionFactory dbConnectionFactory, CancellationToken ct) =>
        {
            await using var connection = await dbConnectionFactory.OpenConnectionAsync();
            
            const string sql = 
                $"""
                 SELECT
                 p.id as {nameof(ProductSummary.Id)},
                 p.name as {nameof(ProductSummary.Name)},
                 p.price as {nameof(ProductSummary.Price)},
                 c.name as {nameof(ProductSummary.Category)}
                 FROM catalog.products p
                 JOIN catalog.categories c ON p.category_id = c.id
                 WHERE p.is_available = true
                 AND(@categoryId is null or c.id = @categoryId)
                 ORDER BY p.name
                 LIMIT @pageSize OFFSET (@pageNumber - 1) * @pageSize;
                 """;

            var products = (await connection.QueryAsync<ProductSummary>(sql, new
            {
                request.CategoryId,
                request.PageSize, 
                request.PageNumber
            })).ToList();

            return products.Any() ? Results.Ok(new GetCatalogResponse(products, products.Count, request.PageNumber))
                                  : Results.Ok("Catalog is empty");
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetCatalog>>() 
        .WithTags("Catalog")
        .WithName("GetCatalog")
        .WithSummary("Get catalog")
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

