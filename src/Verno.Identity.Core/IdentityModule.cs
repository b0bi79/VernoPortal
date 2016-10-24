using System.Reflection;
using Abp.Dependency;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Verno.Identity.Data;
using Verno.Identity.Localization;

namespace Verno.Identity
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    public class IdentityModule : AbpModule
    {
        public static string ApplicationName;

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = IdentityConsts.ConnectionStringName;
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            IdentityLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            //IocManager.Register<IdentityDbContext>(DependencyLifeStyle.Transient);
        }
    }
}