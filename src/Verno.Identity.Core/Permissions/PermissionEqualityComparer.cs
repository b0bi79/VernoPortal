﻿using System.Collections.Generic;
using Abp.Authorization;

namespace Verno.Identity.Permissions
{
    /// <summary>
    /// Equality comparer for <see cref="Permission" /> objects.
    /// </summary>
    internal class PermissionEqualityComparer : IEqualityComparer<Permission>
    {
        public bool Equals(Permission x, Permission y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return Equals(x.Name, y.Name);
        }

        public int GetHashCode(Permission permission)
        {
            return permission.Name.GetHashCode();
        }
    }
}