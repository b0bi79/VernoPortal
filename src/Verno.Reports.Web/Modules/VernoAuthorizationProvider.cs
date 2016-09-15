using Abp.Authorization;

namespace Verno.Reports.Web.Modules
{
    public class VernoAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var documents = context.CreatePermission("Documents");
            documents.CreateChildPermission("Documents.Print");
            documents.CreateChildPermission("Documents.Returns");
        }
    }
}