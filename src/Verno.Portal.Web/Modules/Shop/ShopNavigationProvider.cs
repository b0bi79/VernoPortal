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
            Documents
                .AddItem(
                    new MenuItemDefinition(
                        "production-calculator",
                        L("ProductionCalculator"),
                        requiredPermissionName: ShopPermissionNames.Documents_Shop_Production
                    )
                );
        }
    }

    #endregion
}