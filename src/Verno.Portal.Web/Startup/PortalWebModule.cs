using System.Reflection;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Verno.Reports.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Verno.Configuration;
using Verno.Identity;
using Verno.Portal.Web.Localization;
using Verno.Portal.Web.Modules.Print;
using Verno.Portal.Web.Modules.Returns;
using Verno.Portal.Web.Modules.Shop;
using Verno.Reports;

namespace Verno.Portal.Web.Startup
{
    [DependsOn(
        typeof(IdentityApplicationModule), 
        typeof(ReportsApplicationModule), 
        typeof(ReportsEntityFrameworkCoreModule), 
        typeof(PrintEntityFrameworkCoreModule), 
        typeof(ReturnsEntityFrameworkCoreModule), 
        typeof(AbpAspNetCoreModule))]
    public class PortalWebModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public PortalWebModule(IHostingEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(ReportsConsts.ConnectionStringName);

            PortalLocalizationConfigurer.Configure(Configuration.Localization);

            var config = Configuration.Modules.AbpAspNetCore();
            config.CreateControllersForAppServices(typeof(ReportsApplicationModule).Assembly, "reports");
            config.CreateControllersForAppServices(typeof(IdentityApplicationModule).Assembly, "identity");
            config.CreateControllersForAppServices(typeof(PortalWebModule).Assembly, "app");
            Configuration.Authorization.Providers.Add<ShopAuthorizationProvider>();
            Configuration.Authorization.Providers.Add<PrintAuthorizationProvider>();
            Configuration.Authorization.Providers.Add<ReturnsAuthorizationProvider>();

            Configuration.Navigation.Providers.Add<DashboardNavigationProvider>();
            Configuration.Navigation.Providers.Add<PrintNavigationProvider>();
            Configuration.Navigation.Providers.Add<ReturnsNavigationProvider>();
            Configuration.Navigation.Providers.Add<ShopNavigationProvider>();
            Configuration.Navigation.Providers.Add<IdentityNavigationProvider>();
            Configuration.Navigation.Providers.Add<ReportsNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}