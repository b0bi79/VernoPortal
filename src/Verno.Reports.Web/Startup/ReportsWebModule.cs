using System.Reflection;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Configuration;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Castle.MicroKernel.Registration;
using Verno.Reports.Configuration;
using Verno.Reports.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Verno.Identity;
using Verno.Reports.Web.Configuration;
using Verno.Reports.Web.Modules;
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
            Configuration.Authorization.Providers.Add<VernoAuthorizationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}