using Abp.Authorization;

namespace Verno.Reports
{
    public class ReportsAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context
                .CreatePermission("Products")
                .CreateChildPermission("Products.Read");
        }
    }
}