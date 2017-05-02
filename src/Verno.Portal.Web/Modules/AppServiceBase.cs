using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Runtime.Session;
using Verno.Identity.Users;

namespace Verno.Portal.Web.Modules
{
    public class AppServiceBase : ApplicationService
    {
        public UserManager UserManager { get; set; }

        protected async Task<int> GetUserShopNum()
        {
            var user = await GetCurrentUserAsync();
            return await GetUserShopNum(user);
        }

        protected async Task<int> GetUserShopNum(User user)
        {
            var claims = await UserManager.GetClaimAsync(user, UserClaimTypes.ShopNum);
            if (claims == null)
                return 0;
            return int.Parse(claims.Value);
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }
    }
}