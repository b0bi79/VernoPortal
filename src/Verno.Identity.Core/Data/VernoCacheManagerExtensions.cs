using Abp.Runtime.Caching;
using Verno.Identity.Permissions;
using Verno.Identity.Roles;
using Verno.Identity.Users;

namespace Verno.Identity.Data
{
    public static class VernoCacheManagerExtensions
    {
        public static ITypedCache<long, UserPermissionCacheItem> GetUserPermissionCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<long, UserPermissionCacheItem>(UserPermissionCacheItem.CacheStoreName);
        }

        public static ITypedCache<int, RolePermissionCacheItem> GetRolePermissionCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<int, RolePermissionCacheItem>(RolePermissionCacheItem.CacheStoreName);
        }

        public static ITypedCache<int, ClaimPermissionCacheItem> GetClaimPermissionCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<int, ClaimPermissionCacheItem>(ClaimPermissionCacheItem.CacheStoreName);
        }
    }
}