using Abp.Application.Navigation;
using Abp.Localization;

namespace Verno.Reports
{
    /// <summary>
    /// This class defines menus for the application.
    /// </summary>
    public class ReportsNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        "reports",
                        L("Reports"),
                        url: "",
                        icon: "fa fa-home"
                        )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ReportsConsts.LocalizationSourceName);
        }
    }
}
