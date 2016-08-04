using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verno.Identity.Users
{
    /// <summary>Used to save a login attempt of a user.</summary>
    [Table("UserLoginAttempts")]
    public class UserLoginAttempt : Entity<int>, IHasCreationTime
    {
        /// <summary>
        /// Max length of the <see cref="UserNameOrEmailAddress" /> property.
        /// </summary>
        public const int MaxUserNameOrEmailAddressLength = 256;
        /// <summary>
        /// Maximum length of <see cref="ClientIpAddress" /> property.
        /// </summary>
        public const int MaxClientIpAddressLength = 64;
        /// <summary>
        /// Maximum length of <see cref="ClientName" /> property.
        /// </summary>
        public const int MaxClientNameLength = 128;
        /// <summary>
        /// Maximum length of <see cref="BrowserInfo" /> property.
        /// </summary>
        public const int MaxBrowserInfoLength = 256;
        /// <summary>
        /// Maximum length of <see cref="MaxResultLength" /> property.
        /// </summary>
        public const int MaxResultLength = 32;

        /// <summary>
        /// User's Id, if <see cref="UserNameOrEmailAddress" /> was a valid username or email address.
        /// </summary>
        public virtual int? UserId { get; set; }

        /// <summary>User name or email address</summary>
        [MaxLength(MaxUserNameOrEmailAddressLength)]
        public virtual string UserNameOrEmailAddress { get; set; }

        /// <summary>IP address of the client.</summary>
        [MaxLength(MaxClientIpAddressLength)]
        public virtual string ClientIpAddress { get; set; }

        /// <summary>Name (generally computer name) of the client.</summary>
        [MaxLength(MaxClientNameLength)]
        public virtual string ClientName { get; set; }

        /// <summary>
        /// Browser information if this method is called in a web request.
        /// </summary>
        [MaxLength(MaxBrowserInfoLength)]
        public virtual string BrowserInfo { get; set; }

        /// <summary>Login attempt result.</summary>
        [MaxLength(MaxResultLength)]
        public virtual string Result { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public string Application { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginAttempt" /> class.
        /// </summary>
        public UserLoginAttempt()
        {
            CreationTime = Clock.Now;
        }
    }
}
