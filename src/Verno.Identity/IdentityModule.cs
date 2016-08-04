using System.Reflection;
using Abp.Auditing;
using Abp.EntityFrameworkCore;
using Abp.Modules;

namespace Verno.Identity
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    public class IdentityModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Identity";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}