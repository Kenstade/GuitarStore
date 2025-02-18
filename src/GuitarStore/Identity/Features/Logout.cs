using GuitarStore.Common.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace GuitarStore.Identity.Features;

internal sealed class Logout : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/logout", HandleAsync)
        .RequireAuthorization();
    private static async Task HandleAsync(HttpContext context)
    {
        await context.SignOutAsync();
    }
}
