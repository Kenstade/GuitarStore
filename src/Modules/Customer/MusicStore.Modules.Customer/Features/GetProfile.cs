using System.Security.Claims;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security.Authentication;
using BuildingBlocks.Web.MinimalApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Customer.Data;
using MusicStore.Modules.Customer.Errors;

namespace MusicStore.Modules.Customer.Features;

internal sealed class GetProfile : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/customers/profile", async (CustomerDbContext dbContext, ClaimsPrincipal user) =>
        {
            var userId = user.GetUserId();

            var profile = await dbContext.Customers
                .Where(c => c.Id == userId)
                .Select(c => new GetProfileResponse
                (
                    c.Email,
                    c.PhoneNumber,
                    c.FullName,
                    c.Addresses.Select(a => new AddressSummary
                    (
                        a.City, 
                        a.Street, 
                        a.BuildingNumber, 
                        a.Apartment
                    )).ToList()
                )).FirstOrDefaultAsync();
            
            return profile is not null 
                ? Results.Ok(profile) 
                : Results.Problem(CustomerErrors.NotFound(userId));
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetProfile>>()
        .WithTags("Customers")
        .WithName("GetProfile")
        .WithSummary("Get profile")
        .WithDescription("Get the current user's profile")
        .RequireAuthorization(Constants.Permissions.GetProfile);

        return builder;
    }
}

internal sealed record GetProfileResponse( string Email, string? PhoneNumber, string? FullName, 
    ICollection<AddressSummary> Addresses);
internal sealed record AddressSummary(string City, string Street, string BuildingNumber, string Apartment);