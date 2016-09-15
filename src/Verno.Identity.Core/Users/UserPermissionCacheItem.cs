using System;
using System.Collections.Generic;

namespace Verno.Identity.Users
{
    /// <summary>Used to cache roles and permissions of a user.</summary>
    [Serializable]
    public class UserPermissionCacheItem
    {
        public const string CacheStoreName = "VernoUserPermissions";

        /// <summary>
        /// Gets/sets expire time for cache items.
        /// Default: 20 minutes.
        /// TODO: This is not used yet!
        /// </summary>
        public static TimeSpan CacheExpireTime { get; private set; }

        public int UserId { get; set; }

        public List<int> RoleIds { get; set; }

        public HashSet<string> GrantedPermissions { get; set; }

        public HashSet<string> ProhibitedPermissions { get; set; }

        static UserPermissionCacheItem()
        {
            CacheExpireTime = TimeSpan.FromMinutes(20.0);
        }

        public UserPermissionCacheItem()
        {
            RoleIds = new List<int>();
            GrantedPermissions = new HashSet<string>();
            ProhibitedPermissions = new HashSet<string>();
        }

        public UserPermissionCacheItem(int userId)
          : this()
        {
            UserId = userId;
        }
    }
}