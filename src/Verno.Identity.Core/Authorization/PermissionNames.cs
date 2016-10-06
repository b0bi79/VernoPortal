namespace Verno.Identity.Authorization
{
    public static class PermissionNames
    {
        public const string Administration = "Administration";
        public const string Administration_UserManagement = Administration + ".UserManagement";
        public const string Administration_UserManagement_EditPermissions = Administration_UserManagement + ".EditPermissions";
        public const string Administration_UserManagement_EditRoles = Administration_UserManagement + ".EditRoles";
        public const string Administration_UserManagement_CreateUser = Administration_UserManagement + ".CreateUser";
        public const string Administration_UserManagement_UpdateUser = Administration_UserManagement + ".UpdateUser";
        public const string Administration_UserManagement_DeleteUser = Administration_UserManagement + ".DeleteUser";
        public const string Administration_UserManagement_ResetPassword = Administration_UserManagement + ".ResetPassword";

        public const string Administration_RoleManagement = Administration + ".RoleManagement";
    }
}