using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;

namespace Verno.Identity.Localization
{
    public static class IdentityLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flags england", isDefault: true));
            localizationConfiguration.Languages.Add(new LanguageInfo("ru", "Русский", "famfamfam-flags ru"));

            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(IdentityConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "Verno.Identity.Core.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
