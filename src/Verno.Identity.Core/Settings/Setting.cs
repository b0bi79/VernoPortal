using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.Identity.Settings
{
    /// <summary>Represents a setting for a tenant or user.</summary>
    [Table("UserSettings")]
    public class Setting : Entity<int>
    {
        /// <summary>
        /// Maximum length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxNameLength = 256;
        /// <summary>
        /// Maximum length of the <see cref="Value" /> property.
        /// </summary>
        public const int MaxValueLength = 2000;

        /// <summary>
        /// UserId for this setting.
        /// UserId is null if this setting is not user level.
        /// </summary>
        public virtual int? UserId { get; set; }

        /// <summary>Unique name of the setting.</summary>
        [MaxLength(MaxNameLength)]
        [Required]
        public virtual string Name { get; set; }

        /// <summary>Value of the setting.</summary>
        [MaxLength(MaxValueLength)]
        public virtual string Value { get; set; }

        /// <summary>
        /// Creates a new <see cref="Setting" /> object.
        /// </summary>
        public Setting()
        {
        }

        /// <summary>
        /// Creates a new <see cref="Setting" /> object.
        /// </summary>
        /// <param name="userId">UserId for this setting</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        public Setting(int? userId, string name, string value)
        {
            UserId = userId;
            Name = name;
            Value = value;
        }
    }
}