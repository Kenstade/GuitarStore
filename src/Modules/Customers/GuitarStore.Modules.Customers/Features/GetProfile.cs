using System.Security.Claims;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security.Authentication;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.Customers.Data;
using GuitarStore.Modules.Customers.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Customers.Features;

internal sealed class GetProfile : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/customers/profile", async (CustomersDbContext dbContext, ClaimsPrincipal user) =>
        {
            var userId = user.GetUserId();

            var profile = await dbContext.Customers
                .Where(c => c.Id == userId)
                .Select(c => new GetProfileResponse
                (
                    c.Email,
                    c.PhoneNumber,
                    c.FullName,
                    c.Address.City,
                    c.Address.Street,
                    c.Address.BuildingNumber,
                    c.Address.Apartment
                ))
                .FirstOrDefaultAsync();
            
            return profile is not null 
                ? Results.Ok(profile) 
                : Results.Problem(CustomerErrors.NotFound(userId));
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetProfile>>()
        .WithName("GetProfile")
        .WithTags("Customers")
        .RequireAuthorization(Constants.Permissions.GetProfile);

        return builder;
    }
}

internal sealed record GetProfileResponse(
    string Email, 
    string? PhoneNumber, 
    string? FullName, 
    string? City, 
    string? Street, 
    string? BuildingNumber, 
    string? Apartment);