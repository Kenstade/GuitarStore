using FluentValidation;
using GuitarStore.Common.Interfaces;
using GuitarStore.Data;
using GuitarStore.Identity.Events;
using GuitarStore.Identity.Jwt;
using GuitarStore.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Identity.Features;

public record RegisterRequest(string Email, string Password, string ConfirmPassword);
public class Register : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/SignUp", HandleAsync)
        .AllowAnonymous();

    private static async Task<IResult> HandleAsync(RegisterRequest request,AppDbContext dbContext,
        IValidator<RegisterRequest> validator, JwtService jwtService, UserEventHandler eventHandler,
        PasswordHasher passwordHasher)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
            return TypedResults.ValidationProblem(result.ToDictionary());

        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHasher.Generate(request.Password)
        };

        await dbContext.AddAsync(user);
        await dbContext.SaveChangesAsync();

        await eventHandler.Handle(new UserCreatedEvent(user.Id, user.Email));
     
        return TypedResults.Ok();
    }
}

public class RegisterRequestValidation : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidation(AppDbContext dbContext)
    {
        RuleFor(x => x.Email).EmailAddress().MustAsync(async (email, _) =>
        {
            return !await dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
        }).WithMessage("This email is already exist");

        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password).WithMessage("passwords do not match");
    }
}
