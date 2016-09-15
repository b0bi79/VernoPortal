using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using Abp.Timing;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Verno.Identity.Organizations;
using Verno.Identity.Settings;

namespace Verno.Identity.Users
{
    public class User : User<int>
    {
        /// <inheritdoc />
        public User()
        {
        }

        /// <inheritdoc />
        public User(string userName) : base(userName)
        {
        }
    }

    public abstract class User<TKey> : IdentityUser<TKey, UserClaim, UserRole, IdentityUserLogin<int>>, IPassivable, IEntity<TKey>,
        ICreationAudited, IHasCreationTime where TKey : IEquatable<TKey>
    {
        public const int MinimumPasswordLength = 4;
        public const int MaximumPasswordLength = 30;
        public const string DefaultPassword = "1234";

        /// <inheritdoc />
        protected User()
        {
            CreationTime = Clock.Now;
        }

        /// <inheritdoc />
        protected User(string userName) : base(userName)
        {
            CreationTime = Clock.Now;
        }

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
            if (ReferenceEquals(entity, null))
                return false;
            if (ReferenceEquals(entity, this))
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

        /// <inheritdoc />
        public DateTime CreationTime { get; set; }

        /// <inheritdoc />
        public long? CreatorUserId { get; set; }
    }
}