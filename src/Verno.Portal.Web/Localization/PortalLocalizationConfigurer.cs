using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;

namespace Verno.Portal.Web.Localization
{
    public static class PortalLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Languages.Add(new LanguageInfo("ru", "Русский", "famfamfam-flags ru", isDefault: true));
            localizationConfiguration.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flags england"));

            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(PortalConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "Verno.Portal.Web.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}