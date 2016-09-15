namespace Verno.Identity.Permissions
{
    /// <summary>
    /// Represents a permission <see cref="P:Abp.Authorization.PermissionGrantInfo.Name" /> with <see cref="P:Abp.Authorization.PermissionGrantInfo.IsGranted" /> information.
    /// </summary>
    public class PermissionGrantInfo
    {
        /// <summary>Name of the permission.</summary>
        public string Name { get; private set; }

        /// <summary>Is this permission granted Prohibited?</summary>
        public bool IsGranted { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="PermissionGrantInfo" />.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isGranted"></param>
        public PermissionGrantInfo(string name, bool isGranted)
        {
            this.Name = name;
            this.IsGranted = isGranted;
        }
    }
}