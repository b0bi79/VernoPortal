using System;
using System.Collections.Generic;

namespace Verno.Identity.Users
{
    public class UserClaimTypes
    {
        private static readonly Dictionary<string, ClaimType> ClaimTypes = new Dictionary<string, ClaimType>()
        {
            {"Role",  new ClaimType("Role")},
            {"ShopNum",  new ClaimType("ShopNum")},
            {"BossId",  new ClaimType("BossId")},
            {"Position",  new ClaimType("EmployeePosition", true)}
        };

        public static readonly ClaimType Role = ClaimTypes["Role"];
        public static readonly ClaimType ShopNum = ClaimTypes["ShopNum"];
        public static readonly ClaimType BossId = ClaimTypes["BossId"];
        public static readonly ClaimType Position = ClaimTypes["Position"];

        public static bool IsGlobal(string claimType)
        {
            ClaimType ct;
            return ClaimTypes.TryGetValue(claimType, out ct) && ct.IsGlobal;
        }
    }

    public class ClaimType
    {
        public string Name { get; }
        public bool IsGlobal { get; }
        //public Type ValueType { get; }

        public ClaimType(string name, bool isGlobal = false)
        {
            IsGlobal = isGlobal;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator string(ClaimType dm)
        {
            return dm.ToString();
        }
    }
}