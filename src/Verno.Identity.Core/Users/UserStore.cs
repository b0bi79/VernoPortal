using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Verno.Identity.Permissions;
using Verno.Identity.Roles;

namespace Verno.Identity.Users
{
    public class UserStore : UserStore<User, Role, Data.IdentityDbContext, int, UserClaim, UserRole, 
        IdentityUserLogin<int>, IdentityUserToken<int>>, ITransientDependency, IDisposable, IUserPermissionStore
    {
        private DbSet<UserClaim> UserClaims => Context.UserClaims;
        private DbSet<UserRole> UserRoles => Context.UserRoles;
        private DbSet<Role> Roles => Context.Roles;
        private DbSet<UserPermissionSetting> UserPermissionSettings => Context.UserPermissions;

        public UserStore(Data.IdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        protected override UserRole CreateUserRole(User user, Role role)
        {
            return new UserRole()
            {
                UserId = user.Id,
                RoleId = role.Id
            };
        }

        protected override UserClaim CreateUserClaim(User user, Claim claim)
        {
            var isGlobal = UserClaimTypes.IsGlobal(claim.Type);
            var userClaim = new UserClaim
            {
                UserId = user.Id,
                Application = isGlobal ? null : IdentityModule.ApplicationName,
            };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        protected override IdentityUserLogin<int> CreateUserLogin(User user, UserLoginInfo login)
        {
            return new IdentityUserLogin<int>
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        protected override IdentityUserToken<int> CreateUserToken(User user, string loginProvider, string name, string value)
        {
            return new IdentityUserToken<int>
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }

        public override async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException(nameof(user));

            return await UserClaims
                .Where(uc => uc.UserId == user.Id && (uc.Application == IdentityModule.ApplicationName || uc.Application == null))
                .Select(c => c.ToClaim())
                .ToListAsync(cancellationToken);
        }

        public override async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException(nameof(user));
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            foreach (var claim in claims)
            {
                var matchedClaims = await UserClaims
                    .Where(GetClaimPredicate(user, claim))
                    .ToListAsync(cancellationToken);
                foreach (var c in matchedClaims)
                {
                    UserClaims.Remove(c);
                }
            }
        }

        public override async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));
            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            var matchedClaims = await UserClaims
                .Where(GetClaimPredicate(user, claim))
                .ToListAsync(cancellationToken);

            foreach (var matchedClaim in matchedClaims)
            {
                matchedClaim.ClaimValue = newClaim.Value;
                matchedClaim.ClaimType = newClaim.Type;
            }
        }

        private Expression<Func<UserClaim, bool>> GetClaimPredicate(User user, Claim claim)
        {
            return uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type
                         && (uc.Application == IdentityModule.ApplicationName || uc.Application == null);
        }

        private async Task<Role> GetRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return await Roles.SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName
                                                         && (r.Application == IdentityModule.ApplicationName || r.Application == null), cancellationToken);
        }

        public async Task<IList<string>> GetRolesAsync(int userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var query = from userRole in UserRoles
                        join role in Roles on userRole.RoleId equals role.Id
                        where userRole.UserId == userId && (role.Application == IdentityModule.ApplicationName || role.Application == null)
                        select role.Name;
            return await query.ToListAsync(cancellationToken);
        }

        public override async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await GetRolesAsync(user.Id, cancellationToken);
        }

        public override async Task RemoveFromRoleAsync(User user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentException("Value cannot be null or empty", nameof(normalizedRoleName));

            var roleEntity = await GetRoleAsync(normalizedRoleName, cancellationToken);
            if (roleEntity != null)
            {
                var userRole = await UserRoles.FirstOrDefaultAsync(r => roleEntity.Id == r.RoleId && r.UserId == user.Id, cancellationToken);
                if (userRole != null)
                {
                    UserRoles.Remove(userRole);
                }
            }
        }

        public override async Task<bool> IsInRoleAsync(User user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentException("Value cannot be null or empty", nameof(normalizedRoleName));

            var role = await GetRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                return await UserRoles.AnyAsync(ur => ur.RoleId.Equals(role.Id) && ur.UserId.Equals(user.Id), cancellationToken);
            }
            return false;
        }

        public override async Task<IList<User>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            var role = await GetRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                var query = from userrole in UserRoles
                            join user in Users on userrole.UserId equals user.Id
                            where userrole.RoleId.Equals(role.Id)
                            select user;

                return await query.ToListAsync(cancellationToken);
            }
            return new List<User>();
        }

        #region Implementation of IUserPermissionStore

        /// <inheritdoc />
        public virtual async Task AddPermissionAsync(User user, PermissionGrantInfo permissionGrant)
        {
            if (await HasPermissionAsync(user.Id, permissionGrant))
                return;
            UserPermissionSettings.Add(new UserPermissionSetting
            {
                UserId = user.Id,
                Name = permissionGrant.Name,
                IsGranted = permissionGrant.IsGranted
            });
            await Context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task RemovePermissionAsync(User user, PermissionGrantInfo permissionGrant)
        {
            var toDelete = UserPermissionSettings.Where(
                permissionSetting =>
                    permissionSetting.UserId == user.Id && permissionSetting.Name == permissionGrant.Name &&
                    permissionSetting.IsGranted == permissionGrant.IsGranted);
            UserPermissionSettings.RemoveRange(toDelete);
            await Context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IList<PermissionGrantInfo>> GetPermissionsAsync(int userId)
        {
            return await UserPermissionSettings.Where(p => p.UserId == userId)
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasPermissionAsync(int userId, PermissionGrantInfo permissionGrant)
        {
            return await UserPermissionSettings.FirstOrDefaultAsync(
                       p => p.UserId == userId && p.Name == permissionGrant.Name && p.IsGranted == permissionGrant.IsGranted) != null;
        }

        /// <inheritdoc />
        public async Task RemoveAllPermissionSettingsAsync(User user)
        {
            var toDelete = UserPermissionSettings.Where(s => s.UserId == user.Id);
            UserPermissionSettings.RemoveRange(toDelete);
            await Context.SaveChangesAsync();
        }

        #endregion
    }
}