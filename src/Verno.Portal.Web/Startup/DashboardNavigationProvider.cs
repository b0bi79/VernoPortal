using Abp.Application.Navigation;
using Abp.Localization;
using Verno.Identity.Authorization;

namespace Verno.Portal.Web.Startup
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// </summary>
    public class DashboardNavigationProvider : NavigationProvider
    {
        #region Overrides of NavigationProvider

        /// <inheritdoc />
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                new MenuItemDefinition(
                    "dashboard",
                    L("Dashboard"),
                    "ion-android-home"
                    )
            );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, PortalConsts.LocalizationSourceName);
        }
    }

    #endregion
}