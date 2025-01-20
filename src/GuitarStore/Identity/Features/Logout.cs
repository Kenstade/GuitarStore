using GuitarStore.Common.Interfaces;
using GuitarStore.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GuitarStore.Identity.Features;

public class Logout : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/logout", HandleAsync)
        .RequireAuthorization();
    private static async Task HandleAsync(HttpContext context)
    {
        await context.SignOutAsync();
    }
}
