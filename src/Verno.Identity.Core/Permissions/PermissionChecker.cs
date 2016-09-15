using System.Threading.Tasks;
using Abp;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Verno.Identity.Users;

namespace Verno.Identity.Permissions
{
    public class PermissionChecker: IPermissionChecker, ITransientDependency
    {
        private readonly UserManager _userManager;
        public ILogger Logger { get; set; }
        public IAbpSession AbpSession { get; set; }

        /// <summary>Constructor.</summary>
        public PermissionChecker(UserManager userManager)
        {
            _userManager = userManager;
            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        public virtual async Task<bool> IsGrantedAsync(string permissionName)
        {
            int num;
            if (AbpSession.UserId.HasValue)
                num = await _userManager.IsGrantedAsync((int)AbpSession.UserId.Value, permissionName) ? 1 : 0;
            else
                num = 0;
            return num != 0;
        }

        public virtual async Task<bool> IsGrantedAsync(long userId, string permissionName)
        {
            return await _userManager.IsGrantedAsync((int) userId, permissionName);
        }

        public virtual async Task<bool> IsGrantedAsync(UserIdentifier user, string permissionName)
        {
            return await IsGrantedAsync(user.UserId, permissionName);
        }
    }
}