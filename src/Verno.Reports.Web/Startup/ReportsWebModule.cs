using System.Reflection;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Verno.Reports.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Verno.Configuration;
using Verno.Identity;
using Verno.Reports.Web.Modules.Print;
using Verno.Reports.Web.Modules.Returns;

namespace Verno.Reports.Web.Startup
{
    [DependsOn(
        typeof(IdentityApplicationModule), 
        typeof(ReportsApplicationModule), 
        typeof(ReportsEntityFrameworkCoreModule), 
        typeof(PrintEntityFrameworkCoreModule), 
        typeof(ReturnsEntityFrameworkCoreModule), 
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
            
            var config = Configuration.Modules.AbpAspNetCore();
            config.CreateControllersForAppServices(typeof(ReportsApplicationModule).Assembly, "reports");
            config.CreateControllersForAppServices(typeof(IdentityApplicationModule).Assembly, "identity");
            config.CreateControllersForAppServices(typeof(ReportsWebModule).Assembly, "app");
            Configuration.Authorization.Providers.Add<PrintAuthorizationProvider>();
            Configuration.Authorization.Providers.Add<ReturnsAuthorizationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}