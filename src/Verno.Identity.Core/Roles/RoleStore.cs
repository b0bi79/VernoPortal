using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Verno.Identity.Permissions;
using IdentityDbContext = Verno.Identity.Data.IdentityDbContext;
using System.Linq;
using Abp.Domain.Uow;
using Verno.Identity.Users;

namespace Verno.Identity.Roles
{
    public class RoleStore: RoleStore<Role, IdentityDbContext, int, UserRole, IdentityRoleClaim<int>>, IRolePermissionStore
    {
        private readonly IRepository<RolePermissionSetting, int> _rolePermissionSettingRepository;

        /// <inheritdoc />
        public RoleStore(IdentityDbContext context, IRepository<RolePermissionSetting, int> rolePermissionSettingRepository, 
            IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _rolePermissionSettingRepository = rolePermissionSettingRepository;
        }

        #region Overrides of RoleStore<Role,IdentityDbContext,int,IdentityUserRole<int>,IdentityRoleClaim<int>>

        /// <inheritdoc />
        protected override IdentityRoleClaim<int> CreateRoleClaim(Role role, Claim claim)
        {
            return new IdentityRoleClaim<int> { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
        }

        #endregion

        #region Implementation of IRolePermissionStore

        /// <inheritdoc />
        public virtual async Task AddPermissionAsync(Role role, PermissionGrantInfo permissionGrant)
        {
            if (await HasPermissionAsync(role.Id, permissionGrant))
                return;
            await _rolePermissionSettingRepository.InsertAsync(new RolePermissionSetting
            {
                RoleId = role.Id,
                Name = permissionGrant.Name,
                IsGranted = permissionGrant.IsGranted
            });
        }

        /// <inheritdoc />
        public virtual async Task RemovePermissionAsync(Role role, PermissionGrantInfo permissionGrant)
        {
            await _rolePermissionSettingRepository
                .DeleteAsync(s => s.RoleId == role.Id && s.Name == permissionGrant.Name && s.IsGranted == permissionGrant.IsGranted);
        }

        /// <inheritdoc />
        public virtual Task<IList<PermissionGrantInfo>> GetPermissionsAsync(Role role)
        {
            return GetPermissionsAsync(role.Id);
        }

        [UnitOfWork]
        public async Task<IList<PermissionGrantInfo>> GetPermissionsAsync(int roleId)
        {
            return (await _rolePermissionSettingRepository
                    .GetAllListAsync(p => p.RoleId == roleId))
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToList() as IList<PermissionGrantInfo>;
        }

        /// <inheritdoc />
        public virtual async Task<bool> HasPermissionAsync(int roleId, PermissionGrantInfo permissionGrant)
        {
            return await _rolePermissionSettingRepository
                       .FirstOrDefaultAsync(p => p.RoleId == roleId && p.Name == permissionGrant.Name && p.IsGranted == permissionGrant.IsGranted) != null;
        }

        /// <inheritdoc />
        public virtual async Task RemoveAllPermissionSettingsAsync(Role role)
        {
            await _rolePermissionSettingRepository.DeleteAsync(s => s.RoleId == role.Id);
        }
        #endregion
    }
}