using System.Reflection;
using Abp.EntityFrameworkCore;
using Abp.Modules;

namespace Verno.ShInfo.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule))]
    public class InfoEntityFrameworkCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = ShInfoConsts.ConnectionStringName;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}