using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Abp.Authorization;
using Abp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Verno.Identity.Permissions;
using System.Linq;
using System.Threading;
using Abp.Runtime.Caching;
using Verno.Identity.Data;

namespace Verno.Identity.Roles
{
    public class RoleManager : RoleManager<Role>
    {
        private readonly IPermissionManager _permissionManager;
        private readonly ICacheManager _cacheManager;
        private readonly HttpContext _context;

        /// <inheritdoc />
        public RoleManager(IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger, IHttpContextAccessor contextAccessor, IPermissionManager permissionManager,
            ICacheManager cacheManager)
            : base(store, roleValidators, keyNormalizer, errors, logger, contextAccessor)
        {
            _permissionManager = permissionManager;
            _cacheManager = cacheManager;
            _context = contextAccessor?.HttpContext;
        }

        private IRolePermissionStore RolePermissionStore
        {
            get
            {
                if (!(Store is IRolePermissionStore))
                    throw new AbpException("Store is not IRolePermissionStore");
                return (IRolePermissionStore) Store;
            }
        }

        private CancellationToken CancellationToken
        {
            get
            {
                HttpContext httpContext = this._context;
                if (httpContext == null)
                    return CancellationToken.None;
                return httpContext.RequestAborted;
            }
        }
        /// <summary>Checks if a role is granted for a permission.</summary>
        /// <param name="roleName">The role's name to check it's permission</param>
        /// <param name="permissionName">Name of the permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(string roleName, string permissionName)
        {
            return await IsGrantedAsync((await GetRoleByNameAsync(roleName)).Id, _permissionManager.GetPermission(permissionName));
        }

        /// <summary>Checks if a role has a permission.</summary>
        /// <param name="roleId">The role's id to check it's permission</param>
        /// <param name="permissionName">Name of the permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(int roleId, string permissionName)
        {
            return await IsGrantedAsync(roleId, _permissionManager.GetPermission(permissionName));
        }

        /// <summary>Checks if a role is granted for a permission.</summary>
        /// <param name="role">The role</param>
        /// <param name="permission">The permission</param>
        /// <returns>True, if the role has the permission</returns>
        public Task<bool> IsGrantedAsync(Role role, Permission permission)
        {
            return IsGrantedAsync(role.Id, permission);
        }

        /// <summary>Checks if a role is granted for a permission.</summary>
        /// <param name="roleId">role id</param>
        /// <param name="permission">The permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(int roleId, Permission permission)
        {
            //Get cached role permissions
            var cacheItem = await GetRolePermissionCacheItemAsync(roleId);

            //Check the permission
            return cacheItem.GrantedPermissions.Contains(permission.Name);
        }

        /// <summary>Gets granted permission names for a role.</summary>
        /// <param name="roleId">Role id</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(int roleId)
        {
            return await GetGrantedPermissionsAsync(await GetRoleByIdAsync(roleId));
        }

        /// <summary>Gets granted permission names for a role.</summary>
        /// <param name="roleName">Role name</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(string roleName)
        {
            return await GetGrantedPermissionsAsync(await GetRoleByNameAsync(roleName));
        }

        /// <summary>Gets granted permissions for a role.</summary>
        /// <param name="role">Role</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(Role role)
        {
            var permissionList = new List<Permission>();
            foreach (Permission allPermission in _permissionManager.GetAllPermissions())
            {
                if (await IsGrantedAsync(role.Id, allPermission))
                    permissionList.Add(allPermission);
            }
            return permissionList;
        }

        /// <summary>
        /// Sets all granted permissions of a role at once.
        /// Prohibits all other permissions.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(int roleId, IEnumerable<Permission> permissions)
        {
            await SetGrantedPermissionsAsync(await GetRoleByIdAsync(roleId), permissions);
        }

        /// <summary>
        /// Sets all granted permissions of a role at once.
        /// Prohibits all other permissions.
        /// </summary>
        /// <param name="role">The role</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(Role role, IEnumerable<Permission> permissions)
        {
            var oldPermissions = await GetGrantedPermissionsAsync(role);
            var newPermissions = permissions.ToArray();
            foreach (var permission in oldPermissions.Where(p => !newPermissions.Contains(p, new PermissionEqualityComparer())))
                await ProhibitPermissionAsync(role, permission);
            foreach (var permission in newPermissions.Where(p => !oldPermissions.Contains(p, new PermissionEqualityComparer())))
                await GrantPermissionAsync(role, permission);
        }

        /// <summary>Grants a permission for a role.</summary>
        /// <param name="role">Role</param>
        /// <param name="permission">Permission</param>
        public async Task GrantPermissionAsync(Role role, Permission permission)
        {
            if (await IsGrantedAsync(role.Id, permission))
                return;

            await RolePermissionStore.AddPermissionAsync(role, new PermissionGrantInfo(permission.Name, true));
        }

        /// <summary>Prohibits a permission for a role.</summary>
        /// <param name="role">Role</param>
        /// <param name="permission">Permission</param>
        public async Task ProhibitPermissionAsync(Role role, Permission permission)
        {
            if (!await IsGrantedAsync(role.Id, permission))
                return;

            await RolePermissionStore.RemovePermissionAsync(role, new PermissionGrantInfo(permission.Name, true));
        }

        /// <summary>Prohibits all permissions for a role.</summary>
        /// <param name="role">Role</param>
        public async Task ProhibitAllPermissionsAsync(Role role)
        {
            foreach (Permission allPermission in _permissionManager.GetAllPermissions())
                await ProhibitPermissionAsync(role, allPermission);
        }

        /// <summary>
        /// Resets all permission settings for a role.
        /// It removes all permission settings for the role.
        /// Role will have permissions those have <see cref="P:Abp.Authorization.Permission.IsGrantedByDefault" /> set to true.
        /// </summary>
        /// <param name="role">Role</param>
        public async Task ResetAllPermissionsAsync(Role role)
        {
            await RolePermissionStore.RemoveAllPermissionSettingsAsync(role);
        }

        /// <summary>
        /// Gets a role by given id.
        /// Throws exception if no role with given id.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <returns>Role</returns>
        /// <exception cref="T:Abp.AbpException">Throws exception if no role with given id</exception>
        public virtual async Task<Role> GetRoleByIdAsync(int roleId)
        {
            Role role = await FindByIdAsync(roleId.ToString());
            if (role == null)
                throw new AbpException("There is no role with id: " + (object) roleId);
            return role;
        }

        /// <summary>
        /// Gets a role by given name.
        /// Throws exception if no role with given roleName.
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Role</returns>
        /// <exception cref="T:Abp.AbpException">Throws exception if no role with given roleName</exception>
        public virtual async Task<Role> GetRoleByNameAsync(string roleName)
        {
            Role role = await FindByNameAsync(roleName);
            if (role == null)
                throw new AbpException("There is no role with name: " + roleName);
            return role;
        }

        /// <inheritdoc />
        public override async Task UpdateNormalizedRoleNameAsync(Role role)
        {
            string roleNameAsync = await GetRoleNameAsync(role);
            await Store.SetNormalizedRoleNameAsync(role, NormalizeKey(role.Application, roleNameAsync), CancellationToken);
        }

        /// <inheritdoc />
        public override Task<Role> FindByNameAsync(string roleName)
        {
            return FindByNameAsync(IdentityModule.ApplicationName, roleName);
        }

        public virtual Task<Role> FindByNameAsync(string application, string roleName)
        {
            ThrowIfDisposed();
            if (roleName == null)
                throw new ArgumentNullException(nameof(roleName));
            return Store.FindByNameAsync(NormalizeKey(application, roleName), CancellationToken);
        }

        /// <inheritdoc />
        public override Task<bool> RoleExistsAsync(string roleName)
        {
            return RoleExistsAsync(IdentityModule.ApplicationName, roleName);
        }

        public virtual async Task<bool> RoleExistsAsync(string application, string roleName)
        {
            ThrowIfDisposed();
            if (roleName == null)
                throw new ArgumentNullException(nameof(roleName));
            return await FindByNameAsync(NormalizeKey(application, roleName)) != null;
        }

        /// <inheritdoc />
        public virtual string NormalizeKey(string application, string key)
        {
            return base.NormalizeKey(application) + "_" + base.NormalizeKey(key);
        }

        private async Task<RolePermissionCacheItem> GetRolePermissionCacheItemAsync(int roleId)
        {
            var cacheKey = roleId /*+ (GetCurrentTenantId() ?? 0)*/;
            return await _cacheManager.GetRolePermissionCache().GetAsync(cacheKey, async () =>
            {
                var newCacheItem = new RolePermissionCacheItem(roleId);

                foreach (var permissionInfo in await RolePermissionStore.GetPermissionsAsync(roleId))
                {
                    if (permissionInfo.IsGranted)
                    {
                        newCacheItem.GrantedPermissions.Add(permissionInfo.Name);
                    }
                }

                return newCacheItem;
            });
        }

    }
}