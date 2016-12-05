using Abp.Application.Navigation;
using Abp.Localization;
using Verno.Portal.Web.Modules.Print;

namespace Verno.Portal.Web.Modules
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// </summary>
    public abstract class DocumentsNavigationProvider : NavigationProvider
    {
        protected MenuItemDefinition Documents { get; private set; }

        /// <inheritdoc />
        public override void SetNavigation(INavigationProviderContext context)
        {
            Documents = context.Manager.MainMenu.GetItemByNameOrNull("documents");
            if (Documents == null)
            {
                Documents = new MenuItemDefinition(
                    "documents",
                    L("Documents"),
                    "ion-document",
                    order: 100,
                    customData: new {expanded = true},
                    requiredPermissionName: PrintPermissionNames.Documents
                );
                context.Manager.MainMenu.AddItem(Documents);
            }
        }

        protected static ILocalizableString L(string name)
        {
            return new LocalizableString(name, PortalConsts.LocalizationSourceName);
        }
    }
}