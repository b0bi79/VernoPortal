using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Verno.Identity.Users
{
    public static class UserExtensions
    {
        public static IQueryable<User> GetAllActive(this IQueryable<User> users)
        {
            return users.Where(u => u.IsActive);
        }

        public static async Task<Claim> GetClaimAsync(this UserManager manager, User user, string claimName)
        {
            var claims = await manager.GetClaimsAsync(user);
            return claims.FirstOrDefault(x => x.Type == claimName);
        }
    }
}