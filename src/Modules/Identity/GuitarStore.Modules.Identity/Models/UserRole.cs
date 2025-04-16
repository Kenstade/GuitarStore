using Microsoft.AspNetCore.Identity;

namespace GuitarStore.Modules.Identity.Models;

public class UserRole : IdentityUserRole<Guid>
{
    public virtual User? User { get; set; }
    public virtual Role? Role { get; set; }
}
