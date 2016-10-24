using System;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
#if FX_CORE
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
#endif
#if NETFX_45
using Microsoft.AspNet.Identity.EntityFramework;
#endif

namespace Verno.Identity.Users
{
    public class UserRole : UserRole<int>
    {
    }

    public class UserRole<TKey>: IdentityUserRole<TKey>, ICreationAudited, IHasCreationTime
        where TKey : IEquatable<TKey>
    {
        /// <inheritdoc />
        public DateTime CreationTime { get; set; }

        /// <inheritdoc />
        public long? CreatorUserId { get; set; }

        /// <inheritdoc />
        public UserRole()
        {
            CreationTime = Clock.Now;
        }
    }
}