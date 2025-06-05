using System.Security.Claims;
using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.ErrorHandling;
using BuildingBlocks.Core.Security.Authentication;
using GuitarStore.Modules.Identity.Features;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarStore.Modules.Identity.Authorization;

public sealed class CustomClaimsTransformation(IServiceScopeFactory serviceScopeFactory) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(x => x.Type == CustomClaims.Sub))
        {
            return principal;
        }

        using var scope = serviceScopeFactory.CreateScope();
        
        var queryHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetUserPermissions, GetUserPermissionsResponse>>();
        
        string identityId = principal.GetIdentityId();
        
        var result = await queryHandler.Handle(new GetUserPermissions(identityId));
        
        if(result.IsFailure)
        {
            throw new ProblemDetailsException(result.Error);
        }

        var claimsIdentity = new ClaimsIdentity();
        
        claimsIdentity.AddClaim(new Claim(CustomClaims.Sub, result.Value.UserId.ToString()));

        foreach (var permission in result.Value.Permissions)
        {
            claimsIdentity.AddClaim(new Claim(CustomClaims.Permission, permission));
        }
        principal.AddIdentity(claimsIdentity);
        return principal;
    }
}