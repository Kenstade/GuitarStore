using System.Security.Claims;
using FluentValidation;
using GuitarStore.Common.Interfaces;
using GuitarStore.Identity.Jwt;
using GuitarStore.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace GuitarStore.Identity.Features;

public sealed record RefreshTokenRequest( string AccessToken, string RefreshToken);
public sealed record RefreshTokenResponse(string AccessToken, string RefreshToken);
internal sealed class RefreshToken : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/refresh-token", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync(RefreshTokenRequest request,JwtService jwtService, 
        UserManager<User> userManager, IValidator<RefreshTokenRequest> validator)
    {
        var result = validator.Validate(request);
        if (!result.IsValid) return TypedResults.ValidationProblem(result.ToDictionary());

        var principal = jwtService.GetPrincipalFromToken(request.AccessToken);

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return TypedResults.NotFound();

        string accessToken = jwtService.GenerateJwtToken(user);
        string refreshToken = await jwtService.GenerateRefreshToken(user.Id, request.RefreshToken);

        return TypedResults.Ok(new RefreshTokenResponse(accessToken, refreshToken));
    }
}

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
        RuleFor(x => x.AccessToken).NotEmpty();
    }
}
