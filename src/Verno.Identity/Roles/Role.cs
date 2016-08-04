using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Verno.Identity.Users;

namespace Verno.Identity.Roles
{
    public class Role : IdentityRole<int>
    {
        public string Application { get; set; }

        public override string ToString()
        {
            return $"[Role {Id}, Name={Name}]";
        }
    }
}