using Abp.Authorization;
using Abp.Localization;
using Verno.Identity;

namespace Verno.Portal.Web.Modules.Returns
{
    public class ReturnsAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var documents = context.GetPermissionOrNull(ReturnsPermissionNames.Documents) ??
                                 context.CreatePermission(ReturnsPermissionNames.Documents, L("Documents"));

            var returns = documents.CreateChildPermission(ReturnsPermissionNames.Documents_Returns, L("Returnms "));
            returns.CreateChildPermission(ReturnsPermissionNames.Documents_Returns_GetFile, L("Get a file of return"));
            returns.CreateChildPermission(ReturnsPermissionNames.Documents_Returns_UploadFile, L("Upload a file for the document return"));
            returns.CreateChildPermission(ReturnsPermissionNames.Documents_Returns_DeleteFile, L("Delete a file for the document return"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, IdentityConsts.LocalizationSourceName);
        }
    }
}