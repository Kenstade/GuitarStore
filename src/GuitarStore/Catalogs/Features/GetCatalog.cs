using GuitarStore.Common.Interfaces;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Catalogs.Features;

public sealed record GetCatalogRequest(string? SearchTerm, int PageNumber = 1, int PageSize = 10);
public class GetCatalog : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("", HandleAsync)
        .AllowAnonymous();
    private static async Task<IResult> HandleAsync([AsParameters]GetCatalogRequest request, 
        AppDbContext dbContext, HttpContext context)
    {
        var catalogQuery = dbContext.Products
            .Include(p => p.Category)            
            .AsQueryable();

        var products = await catalogQuery
            .AsNoTrackingWithIdentityResolution()
            .Where(p => p.IsAvailable == true && EF.Functions.Like(p.Name, $"%{request.SearchTerm}%"))
            .OrderBy(p => p.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductPartialResponse
            (
                p.Name,
                p.Image,
                p.Price,
                p.Category.Name
            )).ToListAsync();

        var total = await catalogQuery.AsNoTrackingWithIdentityResolution()
            .Where(p => p.IsAvailable && EF.Functions.Like(p.Name, $"%{request.SearchTerm}%"))
            .CountAsync();

        return TypedResults.Ok(new GetCatalogResponse(products, total, request.PageNumber, request.PageSize));
    }
}
public record GetCatalogResponse(
    ICollection<ProductPartialResponse> Products, 
    int TotalResults, 
    int PageNumber, 
    int PageSize);
public record ProductPartialResponse(
    string Name, 
    string Image, 
    decimal Price, 
    string Category);
