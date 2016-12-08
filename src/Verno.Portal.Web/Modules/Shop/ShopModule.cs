using System.Reflection;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Verno.ShInfo;

namespace Verno.Portal.Web.Modules.Shop
{
    [DependsOn(
        typeof(ShInfoCoreModule),
        typeof(AbpEntityFrameworkCoreModule))]
    public class ShopModule : AbpModule
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