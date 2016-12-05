using Abp.Application.Navigation;
using Abp.Localization;

namespace Verno.Portal.Web.Modules.Returns
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// </summary>
    public class ReturnsNavigationProvider : DocumentsNavigationProvider
    {
        #region Overrides of NavigationProvider

        /// <inheritdoc />
        public override void SetNavigation(INavigationProviderContext context)
        {
            base.SetNavigation(context);
            Documents
                .AddItem(
                    new MenuItemDefinition(
                        "returns",
                        L("Returns"),
                        requiredPermissionName: ReturnsPermissionNames.Documents_Returns
                    )
                );
        }
    }

    #endregion
}