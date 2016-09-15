using System.Reflection;
using Abp.EntityFrameworkCore;
using Abp.Modules;

namespace Verno.Reports.Web.Modules.Returns
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule))]
    public class ReturnsEntityFrameworkCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Returns";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}