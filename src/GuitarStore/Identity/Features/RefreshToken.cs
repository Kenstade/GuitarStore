using FluentValidation;
using GuitarStore.Common;
using GuitarStore.Common.Interfaces;
using GuitarStore.Data;
using GuitarStore.Identity.Jwt;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Identity.Features;

public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record RefreshTokenResponse(string AccessToken, string RefreshToken);
public class RefreshToken : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/refresh-token", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync(RefreshTokenRequest request, AppDbContext dbContext, 
        JwtService jwtService, IUserContextProvider userContext, IValidator<RefreshTokenRequest> validator)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) return TypedResults.ValidationProblem(result.ToDictionary());

        var userId = userContext.GetUserId();

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        string accessToken = jwtService.GenerateJwtToken(user);
        string refreshToken = await jwtService.GenerateRefreshToken(userId, request.RefreshToken);

        return TypedResults.Ok(new RefreshTokenResponse(accessToken, refreshToken));
    }
}

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
