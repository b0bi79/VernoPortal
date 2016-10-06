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

        public Claim Claim { get; set; }

        public HashSet<string> GrantedPermissions { get; set; }

        public HashSet<string> ProhibitedPermissions { get; set; }

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