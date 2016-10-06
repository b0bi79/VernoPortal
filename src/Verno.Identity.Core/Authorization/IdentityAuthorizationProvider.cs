using Abp.Authorization;
using Abp.Localization;

namespace Verno.Identity.Authorization
{
    public class IdentityAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //Common permissions
            var administration = context.GetPermissionOrNull(PermissionNames.Administration) ??
                                 context.CreatePermission(PermissionNames.Administration, L("Administration"));

            var userManagement = administration.CreateChildPermission(PermissionNames.Administration_UserManagement, L("Users"));
            userManagement.CreateChildPermission(PermissionNames.Administration_UserManagement_EditPermissions, L("Edit Permissions"));
            userManagement.CreateChildPermission(PermissionNames.Administration_UserManagement_EditRoles, L("Edit Roles"));
            userManagement.CreateChildPermission(PermissionNames.Administration_UserManagement_CreateUser, L("Create User"));
            userManagement.CreateChildPermission(PermissionNames.Administration_UserManagement_UpdateUser, L("Update User"));
            userManagement.CreateChildPermission(PermissionNames.Administration_UserManagement_DeleteUser, L("Delete User"));
            userManagement.CreateChildPermission(PermissionNames.Administration_UserManagement_ResetPassword, L("Reset Password"));

            var roleManagement = administration.CreateChildPermission(PermissionNames.Administration_RoleManagement, L("Roles"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, IdentityConsts.LocalizationSourceName);
        }
    }
}