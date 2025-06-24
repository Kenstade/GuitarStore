using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MusicStore.Modules.Identity.Data;
using MusicStore.Modules.Identity.KeyCloak;
using MusicStore.Modules.Identity.Models;

namespace MusicStore.Modules.Identity.Features;

internal sealed record RegisterUserRequest(string Email, string Password);
internal sealed class Register : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("users/signup", async (RegisterUserRequest userRequest, IIdentityProvider identityProvider, 
                IdentityContext dbContext, CancellationToken ct) =>
        {
            var result = await identityProvider.RegisterUserAsync(userRequest.Email,"First", "Last", userRequest.Password, ct);

            if (result.IsFailure)
                return Results.Problem(result.Error);
           
            var user = User.Create(userRequest.Email, userRequest.Password, result.Value);

            foreach (var role in user.Roles)
            {
                dbContext.Attach(role);
            }
            
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(ct);
        
            return Results.Ok(user.IdentityId);
        })
        .AddEndpointFilter<LoggingEndpointFilter<Register>>()
        .AddEndpointFilter<ValidationEndpointFilter<RegisterUserRequest>>()
        .WithTags("Identity")
        .WithName("Register")
        .WithSummary("Create new user")
        .AllowAnonymous();
        
        return builder;
    }
}

internal sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}