using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;

namespace Verno.Reports
{
    [DependsOn(
        typeof(ReportsCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ReportsApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}