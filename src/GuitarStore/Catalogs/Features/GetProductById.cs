using GuitarStore.Common.Interfaces;
using GuitarStore.Data;

namespace GuitarStore.Catalogs.Features;

public sealed record GetProductByIdRequest(int Id);
internal sealed class GetProductById : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", HandleAsync);
    private static async Task<IResult> HandleAsync([AsParameters]GetProductByIdRequest request, AppDbContext dbContext)
    {
        var product = await dbContext.Products
            .FindAsync(request.Id);

        if (product == null) return TypedResults.NotFound();

        return TypedResults.Ok(new GetProductByIdResponse
        (
            product.Name, 
            product.Description, 
            product.Image,
            product.Price
        ));
    }
}
public sealed record GetProductByIdResponse(string Name, string Description, string Image, decimal Price);
