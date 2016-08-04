using System.Reflection;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Verno.Reports.Web.Startup;

namespace Verno.Reports.Web.Tests
{
    [DependsOn(
        typeof(ReportsWebModule),
        typeof(AbpAspNetCoreTestBaseModule)
        )]
    public class ReportsWebTestModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}