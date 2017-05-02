using Abp.Authorization;
using Abp.Localization;
using Verno.Identity;

namespace Verno.Portal.Web.Modules.Shop
{
    public class ShopAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var documents = context.GetPermissionOrNull(ShopPermissionNames.Documents) ??
                                 context.CreatePermission(ShopPermissionNames.Documents, L("Documents"));

            var shop = documents.CreateChildPermission(ShopPermissionNames.Documents_Shop, L("Shop documents"));
            shop.CreateChildPermission(ShopPermissionNames.Documents_Shop_Production, L("Own production"))
                .CreateChildPermission(ShopPermissionNames.Documents_Shop_Production_Manage, L("Specifications"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, IdentityConsts.LocalizationSourceName);
        }
    }
}