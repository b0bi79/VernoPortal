using Abp.Application.Navigation;
using Abp.Localization;
using Verno.Identity.Authorization;

namespace Verno.Identity
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// </summary>
    public class IdentityNavigationProvider : NavigationProvider
    {
        #region Overrides of NavigationProvider

        /// <inheritdoc />
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                new MenuItemDefinition(
                    "admin",
                    L("Administration"),
                    icon: "ion-settings",
                    order: 10001,
                    requiredPermissionName: PermissionNames.Administration
                    ).AddItem(
                        new MenuItemDefinition(
                            "users",
                            L("Users"),
                            icon: "ion-person-stalker",
                            requiredPermissionName: PermissionNames.Administration_UserManagement
                            )
                    )
            );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, IdentityConsts.LocalizationSourceName);
        }
    }

    #endregion
}