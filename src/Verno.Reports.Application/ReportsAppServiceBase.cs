using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Identity;
using Verno.Identity;
using Verno.Identity.Users;

namespace Verno.Reports
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class IdentityAppServiceBase : ApplicationService
    {
        public UserManager UserManager { get; set; }
        protected IdentityAppServiceBase()
        {
            LocalizationSourceName = ReportsConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync((int) AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}