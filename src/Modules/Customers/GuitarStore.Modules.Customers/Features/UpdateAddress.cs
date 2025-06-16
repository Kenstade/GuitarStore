using System.Security.Claims;
using BuildingBlocks.Core.ErrorHandling;
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

internal sealed record UpdateAddressRequest(string? City, string? Street, string? BuildingNumber, string? Apartment);

internal sealed class UpdateAddress : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut("/customers/address", async (UpdateAddressRequest request, CustomersDbContext dbContext, 
                ClaimsPrincipal user, CancellationToken ct) =>
        {
            var userId = user.GetUserId();

            var customer = await dbContext.Customers
                .Include(c => c.Address)
                .Where(a => a.Id == userId)
                .FirstOrDefaultAsync(ct);

            if (customer is null) return Results.Problem(CustomerErrors.NotFound(userId));
            if (customer.Address is null) return Results.Problem(Error.NotFound("Address not found."));
            
            customer.UpdateAddress(request.City, request.Street, request.BuildingNumber, request.Apartment);
            
            await dbContext.SaveChangesAsync(ct);

            return Results.Ok();
        })
        .AddEndpointFilter<LoggingEndpointFilter<UpdateAddress>>()
        .AddEndpointFilter<ValidationEndpointFilter<UpdateAddressRequest>>()
        .WithName("UpdateAddress")
        .WithTags("Customers")
        .RequireAuthorization(Constants.Permissions.UpdateAddress);

        return builder;
    }
}

internal sealed class UpdateAddressRequestValidator : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressRequestValidator()
    {
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.Street).MaximumLength(100);
        RuleFor(x => x.BuildingNumber).MaximumLength(50);
        RuleFor(x => x.Apartment).MaximumLength(100);
    }
}