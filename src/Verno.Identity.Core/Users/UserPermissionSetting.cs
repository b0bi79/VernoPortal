using Verno.Identity.Permissions;

namespace Verno.Identity.Users
{
    /// <summary>Used to store setting for a permission for a user.</summary>
    public class UserPermissionSetting : PermissionSetting
    {
        public virtual int UserId { get; set; }
    }
}