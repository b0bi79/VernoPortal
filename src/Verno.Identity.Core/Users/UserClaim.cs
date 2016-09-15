using System;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Verno.Identity.Users
{
    public class UserClaim: Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>, ICreationAudited, IHasCreationTime
    {
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
        public UserClaim(User user)
        {
            UserId = user.Id;
            CreationTime = Clock.Now;
        }
    }
}