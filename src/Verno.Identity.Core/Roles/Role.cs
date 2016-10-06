#if FX_CORE
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
#endif
#if NETFX_45
using Microsoft.AspNet.Identity.EntityFramework;
#endif
using Verno.Identity.Users;

namespace Verno.Identity.Roles
{
#if FX_CORE
    public class Role : IdentityRole<int, UserRole, IdentityRoleClaim<int>>
#endif
#if NETFX_45
    public class Role : IdentityRole<int, UserRole>
#endif
    {
        public string Application { get; set; }

        public override string ToString()
        {
            return $"[Role {Id}, Name={Name}]";
        }
    }
}