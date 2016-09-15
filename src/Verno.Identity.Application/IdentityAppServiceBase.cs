using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Verno.Identity.Users;

namespace Verno.Identity
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class IdentityAppServiceBase : ApplicationService
    {
        protected IdentityAppServiceBase()
        {
            LocalizationSourceName = IdentityConsts.LocalizationSourceName;
        }
 
        public UserManager UserManager { get; set; }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
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

        protected virtual void CheckErrors(IdentityResult[] identityResults)
        {
            identityResults.CheckErrors(LocalizationManager);
        }
    }
}