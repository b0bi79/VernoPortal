using Abp.Authorization;

namespace Verno.Identity
{
    public class IdentityAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var administration = context.CreatePermission("Administration");

            var userManagement = administration.CreateChildPermission("Administration.UserManagement");
            userManagement.CreateChildPermission("Administration.UserManagement.EditPermissions");
            userManagement.CreateChildPermission("Administration.UserManagement.EditRoles");
            userManagement.CreateChildPermission("Administration.UserManagement.CreateUser");
            userManagement.CreateChildPermission("Administration.UserManagement.UpdateUser");
            userManagement.CreateChildPermission("Administration.UserManagement.DeleteUser");
            userManagement.CreateChildPermission("Administration.UserManagement.ResetPassword");

            var roleManagement = administration.CreateChildPermission("Administration.RoleManagement");
        }
    }
}