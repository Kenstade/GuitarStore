using Microsoft.AspNetCore.Authorization;

namespace MusicStore.Modules.Identity.Authorization;

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;