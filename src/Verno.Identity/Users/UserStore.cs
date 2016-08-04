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
using Verno.Identity.Roles;

namespace Verno.Identity.Users
{
    public class UserStore : UserStore<User, Role, Data.IdentityDbContext, int, UserClaim, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityUserToken<int>>, ITransientDependency, IDisposable
    {
        private readonly string _currApplication;

        private DbSet<UserClaim> UserClaims => Context.Set<UserClaim>();
        private DbSet<IdentityUserRole<int>> UserRoles => Context.Set<IdentityUserRole<int>>();
        private DbSet<Role> Roles => Context.Set<Role>();

        public UserStore(string application, Data.IdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _currApplication = application;
        }

        protected override IdentityUserRole<int> CreateUserRole(User user, Role role)
        {
            return new IdentityUserRole<int>()
            {
                UserId = user.Id,
                RoleId = role.Id
            };
        }

        protected override UserClaim CreateUserClaim(User user, Claim claim)
        {
            var userClaim = new UserClaim
            {
                UserId = user.Id,
                Application = _currApplication,
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
                .Where(uc => uc.UserId == user.Id && (uc.Application == _currApplication || uc.Application == null))
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
                         && (uc.Application == _currApplication || uc.Application == null);
        }

        private async Task<Role> GetRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return await Roles.SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName
                                                         && (r.Application == _currApplication || r.Application == null), cancellationToken);
        }

        public override async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var userId = user.Id;
            var query = from userRole in UserRoles
                        join role in Roles on userRole.RoleId equals role.Id
                        where userRole.UserId == userId && (role.Application == _currApplication || role.Application == null)
                        select role.Name;
            return await query.ToListAsync(cancellationToken);
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

        public new void Dispose()
        {
            //this._disposed = true;
        }
    }
}