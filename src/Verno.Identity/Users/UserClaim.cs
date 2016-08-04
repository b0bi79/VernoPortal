namespace Verno.Identity.Users
{
    public class UserClaim: Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>
    {
        public string Application { get; set; }
    }
}