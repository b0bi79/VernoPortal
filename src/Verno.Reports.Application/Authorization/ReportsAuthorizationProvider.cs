using Abp.Authorization;

namespace Verno.Reports.Authorization
{
    public class ReportsAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context
                .CreatePermission(PermissionNames.Reports)
                .CreateChildPermission(PermissionNames.Reports_);
        }
    }
}