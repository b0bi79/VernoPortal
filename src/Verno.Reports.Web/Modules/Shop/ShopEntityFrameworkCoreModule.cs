using System.Reflection;
using Abp.EntityFrameworkCore;
using Abp.Modules;

namespace Verno.Reports.Web.Modules.Shop
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule))]
    public class ShopEntityFrameworkCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Shop";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}