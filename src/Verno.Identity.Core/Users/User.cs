using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using Abp.Timing;
using Verno.Identity.Organizations;
using Verno.Identity.Settings;

#if FX_CORE
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
#endif
#if NETFX_45
using Microsoft.AspNet.Identity.EntityFramework;
#endif


namespace Verno.Identity.Users
{
    public class User : User<int, UserClaim, UserRole, IdentityUserLogin<int>>
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

#if FX_CORE
    public abstract class User<TKey, TClaim, TRole, TLogin> : IdentityUser<TKey, TClaim, TRole, TLogin>, IPassivable, IEntity<TKey>,
        ICreationAudited, IHasCreationTime 
        where TKey : IEquatable<TKey>
        where TLogin : IdentityUserLogin<TKey>
        where TRole : IdentityUserRole<TKey>
        where TClaim : IdentityUserClaim<TKey>
#endif
#if NETFX_45
    public abstract class User<TKey, TClaim, TRole, TLogin> : IdentityUser<TKey, TLogin, TRole, TClaim>, IPassivable, IEntity<TKey>,
        ICreationAudited, IHasCreationTime 
        where TKey : IEquatable<TKey>
        where TLogin : IdentityUserLogin<TKey>
        where TRole : IdentityUserRole<TKey>
        where TClaim : IdentityUserClaim<TKey>
#endif
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
        protected User(string userName) :
#if FX_CORE
            base(userName)
        {
#endif
#if NETFX_45
            base()
        {
            UserName = userName;
#endif
            CreationTime = Clock.Now;
        }

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        /// <inheritdoc />
        public static bool operator ==(User<TKey, TClaim, TRole, TLogin> left, User<TKey, TClaim, TRole, TLogin> right)
        {
            if (Equals(left, null))
                return Equals(right, null);
            return left.Equals(right);
        }

        /// <inheritdoc />
        public static bool operator !=(User<TKey, TClaim, TRole, TLogin> left, User<TKey, TClaim, TRole, TLogin> right)
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
            var entity = obj as User<TKey, TClaim, TRole, TLogin>;
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