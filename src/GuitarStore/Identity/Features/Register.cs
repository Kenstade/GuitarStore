using FluentValidation;
using GuitarStore.Common.Interfaces;
using GuitarStore.Data;
using GuitarStore.Identity.Events;
using GuitarStore.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Identity.Features;

public sealed record RegisterRequest(string Email, string Password, string ConfirmPassword);
internal sealed class Register : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/SignUp", HandleAsync)
        .AllowAnonymous();

    private static async Task<IResult> HandleAsync(RegisterRequest request,AppDbContext dbContext,
        IValidator<RegisterRequest> validator, UserEventHandler eventHandler,
        UserManager<User> userManager)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
            return TypedResults.ValidationProblem(result.ToDictionary());

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            CreatedAt = DateTime.Now,
        };

        var identityResult = await userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded) 
            return TypedResults.BadRequest(identityResult.Errors.Select(e => e.Description));

        var roleResult = await userManager.AddToRoleAsync(user, Constants.Role.User);
        if (!roleResult.Succeeded) 
            return TypedResults.BadRequest(roleResult.Errors.Select(e => e.Description));

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

        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}
