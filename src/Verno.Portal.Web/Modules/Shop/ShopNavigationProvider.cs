using Abp.Application.Navigation;
using Abp.Localization;

namespace Verno.Portal.Web.Modules.Shop
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// </summary>
    public class ShopNavigationProvider : DocumentsNavigationProvider
    {
        #region Overrides of NavigationProvider

        /// <inheritdoc />
        public override void SetNavigation(INavigationProviderContext context)
        {
            base.SetNavigation(context);
 
            var productionRoot = new MenuItemDefinition(
                "production",
                L("OwnProduction"),
                requiredPermissionName: ShopPermissionNames.Documents_Shop_Production,
                customData: new
                {
                    expanded = false,
                    selected = false,
                    selectable = true
                }
            );
            productionRoot.AddItem(new MenuItemDefinition(
                "calculator",
                L("ProductionCalculator"),
                requiredPermissionName: ShopPermissionNames.Documents_Shop_Production
            ));
            productionRoot.AddItem(new MenuItemDefinition(
                "manage",
                L("Specifications"),
                requiredPermissionName: ShopPermissionNames.Documents_Shop_Production_Manage
            ));

            Documents.AddItem(productionRoot);
        }
    }

    #endregion
}