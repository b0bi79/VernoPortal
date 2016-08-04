using System.Reflection;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Verno.Reports.Configuration;
using Verno.Reports.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Verno.Reports.Web.Startup
{
    [DependsOn(
        typeof(ReportsApplicationModule), 
        typeof(ReportsEntityFrameworkCoreModule), 
        typeof(AbpAspNetCoreModule))]
    public class ReportsWebModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public ReportsWebModule(IHostingEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(ReportsConsts.ConnectionStringName);

            Configuration.Navigation.Providers.Add<ReportsNavigationProvider>();

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(ReportsApplicationModule).Assembly
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}