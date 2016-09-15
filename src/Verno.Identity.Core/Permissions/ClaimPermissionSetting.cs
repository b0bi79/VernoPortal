namespace Verno.Identity.Permissions
{
    /// <summary>Used to store setting for a permission for a role.</summary>
    public class ClaimPermissionSetting : PermissionSetting
    {
        /// <summary>Role id.</summary>
        public virtual string ClaimType { get; set; }
        public virtual string ClaimValue { get; set; }
    }
}