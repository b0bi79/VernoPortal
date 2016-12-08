using System.Reflection;
using Abp.Modules;
using Verno.Abp.Dependency;
using Verno.ShInfo.Localization;

namespace Verno.ShInfo
{
    public class ShInfoCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new RepositoryConventionalRegistrar());

            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            InfoLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}