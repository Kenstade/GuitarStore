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
using MusicStore.Modules.Customer.Data;
using MusicStore.Modules.Customer.Errors;

namespace MusicStore.Modules.Customer.Features;

internal sealed record UpdateAddressRequest(
    string AddressId,
    string? City, 
    string? Street, 
    string? BuildingNumber, 
    string? Apartment);

internal sealed class UpdateAddress : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut("/customers/addresses", async (UpdateAddressRequest request, CustomerDbContext dbContext, 
            ClaimsPrincipal user, CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            
            var customer = await dbContext.Customers
                .Include(c => c.Addresses.Where(a => a.Id == Guid.Parse(request.AddressId)))
                .FirstOrDefaultAsync(c => c.Id == userId, ct);

            if (customer is null) return Results.Problem(CustomerErrors.NotFound(userId));
            
            customer.UpdateAddress(
                Guid.Parse(request.AddressId), 
                request.City, 
                request.Street,  
                request.BuildingNumber, 
                request.Apartment);
            
            await dbContext.SaveChangesAsync(ct);

            return Results.Ok();
        })
        .AddEndpointFilter<LoggingEndpointFilter<UpdateAddress>>()
        .AddEndpointFilter<ValidationEndpointFilter<UpdateAddressRequest>>()
        .WithTags("Customers")
        .WithName("UpdateAddress")
        .WithSummary("Update address by ID")
        .WithDescription("Update existing address for the current user by ID")
        .RequireAuthorization(Constants.Permissions.UpdateAddress);

        return builder;
    }
}

internal sealed class UpdateAddressRequestValidator : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressRequestValidator()
    {
        RuleFor(p => p.AddressId).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid addressId");
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.Street).MaximumLength(100);
        RuleFor(x => x.BuildingNumber).MaximumLength(50);
        RuleFor(x => x.Apartment).MaximumLength(100);
    }
}