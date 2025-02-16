using System.Collections.Immutable;
using FluentValidation;
using GuitarStore.Common.Interfaces;
using GuitarStore.Data;
using GuitarStore.Identity.Jwt;
using GuitarStore.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace GuitarStore.Identity.Features;

public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResponse(string AccessToken, string RefreshToken);
public class Login : IEndpoint
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
        if (user == null) return TypedResults.BadRequest("Incorrect E-mail or Password");
        
        var signinResult = await signinManager.CheckPasswordSignInAsync(user, request.Password, false);
        if(!signinResult.Succeeded) 
            return TypedResults.BadRequest("Incorrect E-Mail or Password");

        var refreshToken = await jwtService.GenerateRefreshToken(user.Id);

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtService.GenerateJwtToken(user, roles.ToImmutableList());
       
        return TypedResults.Ok(new LoginResponse(accessToken, refreshToken));
    }
}

public class LoginRequestValidation : AbstractValidator<LoginRequest>
{
    public LoginRequestValidation()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
