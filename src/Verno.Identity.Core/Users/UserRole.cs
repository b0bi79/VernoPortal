using System;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Verno.Identity.Users
{
    public class UserRole: IdentityUserRole<int>, ICreationAudited, IHasCreationTime
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