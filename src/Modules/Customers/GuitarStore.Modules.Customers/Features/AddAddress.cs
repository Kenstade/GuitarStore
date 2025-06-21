using System.Security.Claims;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security.Authentication;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.Customers.Data;
using GuitarStore.Modules.Customers.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Customers.Features;

internal sealed record AddAddressRequest(string City, string Street, string BuildingNumber, string Apartment);

internal sealed class AddAddress : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("customers/address", async (AddAddressRequest request, CustomersDbContext dbContext, 
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
        .WithName("SetAddress")
        .WithTags("Customers")
        .RequireAuthorization(Constants.Permissions.SetAddress);

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