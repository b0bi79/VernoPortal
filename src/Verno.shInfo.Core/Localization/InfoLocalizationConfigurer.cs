using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;

namespace Verno.ShInfo.Localization
{
    public static class InfoLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flags england"));
            localizationConfiguration.Languages.Add(new LanguageInfo("ru", "Русский", "famfamfam-flags ru", isDefault: true));

            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(ShInfoConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "Verno.ShInfo.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}