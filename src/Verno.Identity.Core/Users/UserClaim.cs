using System;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

#if FX_CORE
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
#endif
#if NETFX_45
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
#endif

namespace Verno.Identity.Users
{
    public class UserClaim : UserClaim<int>
    {
    }

#if FX_CORE
    public class UserClaim<TKey>: Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<TKey>, ICreationAudited, IHasCreationTime
        where TKey : IEquatable<TKey>
    {
#endif
#if NETFX_45
    public class UserClaim<TKey> : Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<TKey>, ICreationAudited, IHasCreationTime
        where TKey : IEquatable<TKey>
    {
        public Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue);
        }
#endif
        public string Application { get; set; }

        /// <inheritdoc />
        public DateTime CreationTime { get; set; }
        
        /// <inheritdoc />
        public long? CreatorUserId { get; set; }

        /// <inheritdoc />
        public UserClaim()
        {
            CreationTime = Clock.Now;
        }

        /// <inheritdoc />
        public UserClaim(User<TKey, UserClaim<TKey>, UserRole<TKey>, IdentityUserLogin<TKey>>  user)
        {
            UserId = user.Id;
            CreationTime = Clock.Now;
        }
    }
}