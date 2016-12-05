using Abp.Authorization;
using Abp.Localization;
using Verno.Identity;

namespace Verno.Portal.Web.Modules.Print
{
    public class PrintAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var documents = context.GetPermissionOrNull(PrintPermissionNames.Documents) ??
                                 context.CreatePermission(PrintPermissionNames.Documents, L("Documents"));

            var print = documents.CreateChildPermission(PrintPermissionNames.Documents_Print, L("Printed documents"));
            print.CreateChildPermission(PrintPermissionNames.Documents_Print_GetFile, L("Get file of printed document"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, IdentityConsts.LocalizationSourceName);
        }
    }
}