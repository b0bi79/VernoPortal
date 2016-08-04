using System.Reflection;
using Abp.EntityFrameworkCore;
using Abp.Modules;

namespace Verno.Reports.EntityFrameworkCore
{
    [DependsOn(
        typeof(ReportsCoreModule), 
        typeof(AbpEntityFrameworkCoreModule))]
    public class ReportsEntityFrameworkCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}