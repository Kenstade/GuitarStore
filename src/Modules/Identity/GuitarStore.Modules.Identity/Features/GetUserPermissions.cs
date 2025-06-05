using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.ErrorHandling;
using GuitarStore.Modules.Identity.Data;
using GuitarStore.Modules.Identity.Errors;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Identity.Features;

public sealed record GetUserPermissions(string IdentityId) : IQuery<GetUserPermissionsResponse>;
public sealed record GetUserPermissionsResponse(Guid UserId, HashSet<string> Permissions);

internal sealed class GetUserPermissionsHandler(IdentityContext dbContext)
    : IQueryHandler<GetUserPermissions, GetUserPermissionsResponse>
{
    public async Task<Result<GetUserPermissionsResponse>> Handle(GetUserPermissions request, CancellationToken cancellationToken = default)
    {
        var response = await dbContext.Users
            .Where(u => u.IdentityId == request.IdentityId)
            .Select(u => new GetUserPermissionsResponse
            (
                u.Id,
                u.Roles.SelectMany(r => r.Permissions).Select(p => p.Code).Distinct().ToHashSet()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return response ?? Result.Failure<GetUserPermissionsResponse>(UserErrors.NotFound(request.IdentityId));
    }
}