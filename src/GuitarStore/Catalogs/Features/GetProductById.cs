using GuitarStore.Common.Interfaces;
using GuitarStore.Data;

namespace GuitarStore.Catalogs.Features;

public record GetProductByIdRequest(int Id);
public class GetProductById : IEndpoint
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
public record GetProductByIdResponse(string Name, string Description, string Image, decimal Price);
