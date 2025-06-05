using Microsoft.AspNetCore.Authorization;

namespace GuitarStore.Modules.Identity.Authorization;

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;