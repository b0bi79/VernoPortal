using Abp.Authorization;

namespace Verno.Reports.Web.Modules
{
    public class VernoAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var documents = context.CreatePermission("Documents");
            var print = documents.CreateChildPermission("Documents.Print");
            print.CreateChildPermission("Documents.Print.GetFile");

            var returns = documents.CreateChildPermission("Documents.Returns");
            returns.CreateChildPermission("Documents.Returns.GetFile");
            returns.CreateChildPermission("Documents.Returns.UploadFile");
            returns.CreateChildPermission("Documents.Returns.DeleteFile");
        }
    }
}