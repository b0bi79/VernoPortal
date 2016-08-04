using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Verno.Identity.Organizations;

namespace Verno.Identity.Users
{
    public class User : User<int>
    {
    }

    public abstract class User<TKey> : IdentityUser<TKey, UserClaim, IdentityUserRole<int>, IdentityUserLogin<int>>, IPassivable, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        #region Constants

        public const string EmailRegEx = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

        #endregion

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        /// <inheritdoc />
        public static bool operator ==(User<TKey> left, User<TKey> right)
        {
            if (Equals(left, null))
                return Equals(right, null);
            return left.Equals(right);
        }

        /// <inheritdoc />
        public static bool operator !=(User<TKey> left, User<TKey> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Checks if this entity is transient (it has not an Id).
        /// </summary>
        /// <returns>True, if this entity is transient</returns>
        public virtual bool IsTransient()
        {
            return EqualityComparer<TKey>.Default.Equals(Id, default(TKey));
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var entity = obj as User<TKey>;
            if (entity == null)
                return false;
            if (this == entity)
                return true;
            if (IsTransient() && entity.IsTransient())
                return false;
            Type type1 = GetType();
            Type type2 = entity.GetType();
            if (!type1.IsAssignableFrom(type2) && !type2.IsAssignableFrom(type1))
                return false;
            return Id.Equals((object)entity.Id);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        // ProjectID
        public int OrgUnitId { get; set; }
        public OrgUnit OrgUnit { get; set; }

        /// <summary>Name of the user.</summary>
        public virtual string Name { get; set; }

        /// <summary>The last time this user entered to the system.</summary>
        public virtual DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// Is this user active?
        /// If as user is not active, he/she can not use the application.
        /// </summary>
        public virtual bool IsActive { get; set; } = true;

        /// <summary>Settings for this user.</summary>
        [ForeignKey("UserId")]
        public virtual ICollection<Setting> Settings { get; set; }

        public override string ToString()
        {
            return $"[User {Id}] {UserName}";
        }
    }
}