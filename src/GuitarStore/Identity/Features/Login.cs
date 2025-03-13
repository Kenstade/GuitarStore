using System.Collections.Immutable;
using FluentValidation;
using GuitarStore.Common.Web;
using GuitarStore.Data;
using GuitarStore.Identity.Errors;
using GuitarStore.Identity.Jwt;
using GuitarStore.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace GuitarStore.Identity.Features;

public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResponse(string AccessToken, string RefreshToken);
internal sealed class Login : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/login", HandleAsync)
        .AllowAnonymous();
    private static async Task<IResult> HandleAsync(LoginRequest request, AppDbContext dbContext,
        IValidator<LoginRequest> validator, JwtService jwtService, SignInManager<User> signinManager,
        UserManager<User> userManager)
    {
        var result = validator.Validate(request);
        if (!result.IsValid) return TypedResults.ValidationProblem(result.ToDictionary());

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null) return TypedResults.Problem(new InvalidCredentialsError());
        
        var signinResult = await signinManager.CheckPasswordSignInAsync(user, request.Password, false);
        if(!signinResult.Succeeded) return TypedResults.Problem(new InvalidCredentialsError());

        var refreshToken = await jwtService.GenerateRefreshToken(user.Id);

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtService.GenerateJwtToken(user, roles.ToImmutableList());
       
        return TypedResults.Ok(new LoginResponse(accessToken, refreshToken));
    }
}



public sealed class LoginRequestValidation : AbstractValidator<LoginRequest>
{
    public LoginRequestValidation()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
