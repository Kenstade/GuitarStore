using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.Ordering.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GuitarStore.Modules.Ordering.Features;
//Отдельный модуль?
internal sealed class Checkout : IEndpoint
{
    private readonly OrdersDbContext _dbContext;
    public Checkout(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/checkout", async (CancellationToken ct) =>
        {
            return await Handle(ct);
        })
        .RequireAuthorization();    
        
        return builder;
    }

    private async Task<IResult> Handle(CancellationToken ct)
    {
        
        
        
        return TypedResults.Ok();
    }
}