using BuildingBlocks.Core.ErrorHandling;
using BuildingBlocks.Core.Security.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace MusicStore.Modules.Identity.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        HashSet<string> permissions = context.User.GetPermissions();

        if (!permissions.Contains(requirement.Permission))
        {
            throw new ProblemDetailsException(Error.Forbidden("Insufficient permissions for this action"));
        }
        
        context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
}