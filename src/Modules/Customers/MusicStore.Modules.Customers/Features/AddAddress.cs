using System.Security.Claims;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security.Authentication;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Customers.Data;
using MusicStore.Modules.Customers.Errors;

namespace MusicStore.Modules.Customers.Features;

internal sealed record AddAddressRequest(string City, string Street, string BuildingNumber, string Apartment);

internal sealed class AddAddress : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("customers/addresses", async (AddAddressRequest request, CustomersDbContext dbContext, 
            ClaimsPrincipal user, CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            
            var customer = await dbContext.Customers
                .Where(c => c.Id == userId)
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(ct);

            if (customer is null) return Results.Problem(CustomerErrors.NotFound(userId));
            
            customer.AddAddress(request.City, request.Street, request.BuildingNumber, request.Apartment);
            await dbContext.SaveChangesAsync(ct);

            return Results.Ok();
        })
        .AddEndpointFilter<LoggingEndpointFilter<AddAddress>>()
        .AddEndpointFilter<ValidationEndpointFilter<AddAddressRequest>>()
        .WithTags("Customers")
        .WithName("AddAddress")
        .WithSummary("Add new address")
        .WithDescription("Add a new address for the current user")
        .RequireAuthorization(Constants.Permissions.AddAddress);

        return builder;
    }
}

internal sealed class AddAddressRequestValidator : AbstractValidator<AddAddressRequest>
{
    public AddAddressRequestValidator()
    {
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BuildingNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Apartment).NotEmpty().MaximumLength(100);
    }
}