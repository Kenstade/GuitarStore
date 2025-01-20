using FluentValidation;
using GuitarStore.Common.Interfaces;
using GuitarStore.Data;
using GuitarStore.Identity.Jwt;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Identity.Features;

public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResponse(string AccessToken, string RefreshToken);
public class Login : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/login", HandleAsync)
        .AllowAnonymous();
    private static async Task<IResult> HandleAsync(LoginRequest request, AppDbContext dbContext,
        IValidator<LoginRequest> validator, JwtService jwtService, PasswordHasher passwordHasher, HttpContext htt)
    {
        var result = validator.Validate(request);
        if (!result.IsValid) return TypedResults.ValidationProblem(result.ToDictionary());

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return TypedResults.BadRequest("Incorrect E-mail or Password");

        if(!passwordHasher.Verify(request.Password, user.PasswordHash)) 
            return TypedResults.BadRequest("Incorrect E-Mail or Password");

        var accessToken = jwtService.GenerateJwtToken(user);

        var refreshToken = await jwtService.GenerateRefreshToken(user.Id);
       
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
