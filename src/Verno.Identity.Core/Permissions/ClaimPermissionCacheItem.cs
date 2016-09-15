using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Verno.Identity.Permissions
{
    /// <summary>Used to cache permissions of a role.</summary>
    [Serializable]
    public class ClaimPermissionCacheItem
    {
        public const string CacheStoreName = "VernoClaimPermissions";

        /// <summary>
        /// Gets/sets expire time for cache items.
        /// Default: 20 minutes.
        /// TODO: This is not used yet!
        /// </summary>
        public static TimeSpan CacheExpireTime { get; private set; }

        public Claim Claim { get; set; }

        public HashSet<string> GrantedPermissions { get; set; }

        public HashSet<string> ProhibitedPermissions { get; set; }

        static ClaimPermissionCacheItem()
        {
            CacheExpireTime = TimeSpan.FromMinutes(120.0);
        }

        public ClaimPermissionCacheItem()
        {
            GrantedPermissions = new HashSet<string>();
            ProhibitedPermissions = new HashSet<string>();
        }

        public ClaimPermissionCacheItem(Claim claim)
          : this()
        {
            Claim = claim;
        }
    }
}