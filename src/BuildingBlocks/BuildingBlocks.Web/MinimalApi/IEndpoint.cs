using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Web.MinimalApi;
public interface IEndpoint
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}
