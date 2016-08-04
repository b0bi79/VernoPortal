using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Extensions;
using Verno.Identity.Users;

namespace Verno.Identity.Organizations
{
    public class OrgUnit : Entity<int>
    {
        /// <summary>
        /// Maximum length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxNameLength = 32;
        /// <summary>
        /// Maximum length of the <see cref="Code" /> property.
        /// </summary>
        public const int MaxCodeLength = 128;

        /// <summary>Unique name of this role.</summary>
        [StringLength(MaxNameLength)]
        [Required]
        public virtual string Name { get; set; }

        [Column("OrgUnitType")]
        [Required]
        public string OrgUnitType
        {
            get { return Type.ToString().ToUpper(); }
            set { Type = ParseEnum<OrgUnitType>(value); }
        }

        [NotMapped]
        public OrgUnitType Type { get; set; }

        public int? ParentUnitId { get; set; }
        public OrgUnit ParentUnit { get; set; }

        /// <summary>
        /// Hierarchical Code of this organization unit.
        /// Example: "00001.00042.00005".
        /// This is a unique code for a Tenant.
        /// It's changeable if OU hierarch is changed.
        /// </summary>
        [StringLength(MaxCodeLength)]
        [Required]
        public virtual string Code { get; set; }

        /// <summary>Children of this OU.</summary>
        [ForeignKey("ParentUnitId")]
        public virtual ICollection<OrgUnit> Children { get; set; }

        /// <summary>Users of this OU.</summary>
        [ForeignKey("OrgUnitId")]
        public virtual ICollection<User> Users { get; set; }

        public OrgUnit()
        {
            this.Name = Guid.NewGuid().ToString("N");
        }

        public OrgUnit(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"[OrgUnit {Id}, Name={Name}]";
        }

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Creates code for given numbers.
        /// Example: if numbers are 4,2 then returns "00004.00002";
        /// </summary>
        /// <param name="numbers">Numbers</param>
        public static string CreateCode(params int[] numbers)
        {
            if (numbers.IsNullOrEmpty())
                return null;
            return numbers.Select(number => number.ToString(new string('0', 5))).JoinAsString(".");
        }

        /// <summary>
        /// Appends a child code to a parent code.
        /// Example: if parentCode = "00001", childCode = "00042" then returns "00001.00042".
        /// </summary>
        /// <param name="parentCode">Parent code. Can be null or empty if parent is a root.</param>
        /// <param name="childCode">Child code.</param>
        public static string AppendCode(string parentCode, string childCode)
        {
            if (childCode.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(childCode), "childCode can not be null or empty.");
            if (parentCode.IsNullOrEmpty())
                return childCode;
            return parentCode + "." + childCode;
        }

        /// <summary>
        /// Gets relative code to the parent.
        /// Example: if code = "00019.00055.00001" and parentCode = "00019" then returns "00055.00001".
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parentCode">The parent code.</param>
        public static string GetRelativeCode(string code, string parentCode)
        {
            if (code.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            if (parentCode.IsNullOrEmpty())
                return code;
            if (code.Length == parentCode.Length)
                return null;
            return code.Substring(parentCode.Length + 1);
        }

        /// <summary>
        /// Calculates next code for given code.
        /// Example: if code = "00019.00055.00001" returns "00019.00055.00002".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string CalculateNextCode(string code)
        {
            if (code.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            return AppendCode(GetParentCode(code), CreateCode(Convert.ToInt32(GetLastUnitCode(code)) + 1));
        }

        /// <summary>
        /// Gets the last unit code.
        /// Example: if code = "00019.00055.00001" returns "00001".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string GetLastUnitCode(string code)
        {
            if (code.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            string[] strArray = code.Split('.');
            return strArray[strArray.Length - 1];
        }

        /// <summary>
        /// Gets parent code.
        /// Example: if code = "00019.00055.00001" returns "00019.00055".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string GetParentCode(string code)
        {
            if (code.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            string[] strArray = code.Split('.');
            if (strArray.Length == 1)
                return null;
            return strArray.Take(strArray.Length - 1).JoinAsString(".");
        }
    }

    public enum OrgUnitType
    {
        Unknown,
        Filial,
        Project,
        Shop
    }
}