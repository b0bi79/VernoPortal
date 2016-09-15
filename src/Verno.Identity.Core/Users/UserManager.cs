using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Verno.Identity.Permissions;
using System.Linq;
using Abp;
using Abp.Runtime.Caching;
using Verno.Identity.Data;
using Verno.Identity.Roles;

namespace Verno.Identity.Users
{
    public class UserManager : UserManager<User>
    {
        private readonly IPermissionManager _permissionManager;
        private readonly ICacheManager _cacheManager;

        public FeatureDependencyContext FeatureDependencyContext { get; set; }

        public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,
            IPermissionManager permissionManager, ICacheManager cacheManager, RoleManager roleManager)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _permissionManager = permissionManager;
            _cacheManager = cacheManager;
            AbpSession = NullAbpSession.Instance;
            RoleManager = roleManager;
        }

        public IAbpSession AbpSession { get; set; }
        protected RoleManager RoleManager { get; }

        private IUserPermissionStore UserPermissionStore
        {
            get
            {
                if (!(Store is IUserPermissionStore))
                    throw new AbpException("Store is not IUserPermissionStore");
                return (IUserPermissionStore)Store;
            }
        }

        /// <summary>Check whether a user is granted for a permission.</summary>
        /// <param name="userId">User id</param>
        /// <param name="permissionName">Permission name</param>
        public virtual async Task<bool> IsGrantedAsync(int userId, string permissionName)
        {
            return await IsGrantedAsync(userId, _permissionManager.GetPermission(permissionName));
        }

        /// <summary>Check whether a user is granted for a permission.</summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        public virtual Task<bool> IsGrantedAsync(User user, Permission permission)
        {
            return IsGrantedAsync(user.Id, permission);
        }

        /// <summary>Check whether a user is granted for a permission.</summary>
        /// <param name="userId">User id</param>
        /// <param name="permission">Permission</param>
        public virtual async Task<bool> IsGrantedAsync(int userId, Permission permission)
        {
            if (!permission.MultiTenancySides.HasFlag(AbpSession.MultiTenancySide))
                return false;
            if (permission.FeatureDependency != null && AbpSession.MultiTenancySide == MultiTenancySides.Tenant)
            {
                if (!await permission.FeatureDependency.IsSatisfiedAsync(FeatureDependencyContext))
                    return false;
            }
            UserPermissionCacheItem cacheItem = await GetUserPermissionCacheItemAsync(userId);
            if (cacheItem.GrantedPermissions.Contains(permission.Name))
                return true;
            if (cacheItem.ProhibitedPermissions.Contains(permission.Name))
                return false;
            foreach (int roleId in cacheItem.RoleIds)
            {
                if (await RoleManager.IsGrantedAsync(roleId, permission))
                    return true;
            }
            return false;
        }

        /// <summary>Gets granted permissions for a user.</summary>
        /// <param name="user">Role</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(User user)
        {
            var permissionList = new List<Permission>();
            foreach (var allPermission in _permissionManager.GetAllPermissions())
            {
                if (await IsGrantedAsync(user.Id, allPermission))
                    permissionList.Add(allPermission);
            }
            return permissionList;
        }

        /// <summary>
        /// Sets all granted permissions of a user at once.
        /// Prohibits all other permissions.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(User user, IEnumerable<Permission> permissions)
        {
            var oldPermissions = await GetGrantedPermissionsAsync(user);
            var newPermissions = permissions.ToArray();
            foreach (var permission in oldPermissions.Where(p => !newPermissions.Contains(p)))
                await ProhibitPermissionAsync(user, permission);
            foreach (var permission in newPermissions.Where(p => !oldPermissions.Contains(p)))
                await GrantPermissionAsync(user, permission);
        }

        /// <summary>Prohibits all permissions for a user.</summary>
        /// <param name="user">User</param>
        public async Task ProhibitAllPermissionsAsync(User user)
        {
            foreach (Permission allPermission in _permissionManager.GetAllPermissions())
                await ProhibitPermissionAsync(user, allPermission);
        }

        /// <summary>
        /// Resets all permission settings for a user.
        /// It removes all permission settings for the user.
        /// User will have permissions according to his roles.
        /// This method does not prohibit all permissions.
        /// For that, use <see cref="M:Abp.Authorization.Users.AbpUserManager`3.ProhibitAllPermissionsAsync(`2)" />.
        /// </summary>
        /// <param name="user">User</param>
        public async Task ResetAllPermissionsAsync(User user)
        {
            await UserPermissionStore.RemoveAllPermissionSettingsAsync(user);
        }

        /// <summary>
        /// Grants a permission for a user if not already granted.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        public virtual async Task GrantPermissionAsync(User user, Permission permission)
        {
            await UserPermissionStore.RemovePermissionAsync(user, new PermissionGrantInfo(permission.Name, false));
            if (await IsGrantedAsync(user.Id, permission))
                return;
            await UserPermissionStore.AddPermissionAsync(user, new PermissionGrantInfo(permission.Name, true));
        }

        /// <summary>Prohibits a permission for a user if it's granted.</summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        public virtual async Task ProhibitPermissionAsync(User user, Permission permission)
        {
            await UserPermissionStore.RemovePermissionAsync(user, new PermissionGrantInfo(permission.Name, true));
            if (!await IsGrantedAsync(user.Id, permission))
                return;
            await UserPermissionStore.AddPermissionAsync(user, new PermissionGrantInfo(permission.Name, false));
        }

        public Task<User> FindByIdAsync(int userId)
        {
            return base.FindByIdAsync(userId.ToString());
        }

        private async Task<UserPermissionCacheItem> GetUserPermissionCacheItemAsync(int userId)
        {
            return await _cacheManager.GetUserPermissionCache().GetAsync(userId, (async () =>
            {
                var newCacheItem = new UserPermissionCacheItem(userId);
                var user = await FindByIdAsync(userId.ToString());
                foreach (string roleName in await GetRolesAsync(user))
                {
                    newCacheItem.RoleIds.Add((await RoleManager.GetRoleByNameAsync(roleName)).Id);
                }
                foreach (var permissionGrantInfo in await UserPermissionStore.GetPermissionsAsync(userId))
                {
                    if (permissionGrantInfo.IsGranted)
                        newCacheItem.GrantedPermissions.Add(permissionGrantInfo.Name);
                    else
                        newCacheItem.ProhibitedPermissions.Add(permissionGrantInfo.Name);
                }
                return newCacheItem;
            }));
        }

        public async Task<IdentityResult[]> UpdateRolesAsync(User user, string[] roleNames)
        {
            var roles = await GetRolesAsync(user);

            var newItems = roleNames.Except(roles);
            var deletedItems = roles.Except(roleNames);

            var tasks = deletedItems.Select(item => RemoveFromRoleAsync(user, item))
                .Concat(newItems.Select(item => AddToRoleAsync(user, item)));

            var db = ((UserStore) Store).Context.Database;
            using (var tran = await db.BeginTransactionAsync())
            {
                var result = await Task.WhenAll(tasks);
                tran.Commit();
                return result;
            }
        }
    }
}