using GuitarStore.Catalogs.Errors;
using GuitarStore.Common.Caching;
using GuitarStore.Common.Web;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Catalogs.Features;

public sealed record GetProductByIdRequest(int Id);
internal sealed class GetProductById : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", HandleAsync);
    private static async Task<IResult> HandleAsync([AsParameters]GetProductByIdRequest request, 
        AppDbContext dbContext)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new GetProductByIdResponse
            (
                p.Name,
                p.Description,
                p.Image,
                p.Price,
                p.Category.Name,
                p.Brand.Name,
                p.ProductSpecifications
                .Select(ps => new ProductSpecPartialResponse
                (
                    ps.SpecificationType.Name, 
                    ps.Value
                )).ToList()
            )).FirstOrDefaultAsync();

        return product != null ? TypedResults.Ok(product)
                               : TypedResults.Problem(new ProductNotFoundError(request.Id));
    }
}
public sealed record GetProductByIdResponse(
    string Name, 
    string Description, 
    string Image, 
    decimal Price,
    string Category,
    string Brand,
    ICollection<ProductSpecPartialResponse> Specs);
public sealed record ProductSpecPartialResponse(string Type, string Value);
