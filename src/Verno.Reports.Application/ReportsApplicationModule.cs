using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using Verno.Reports.Authorization;

namespace Verno.Reports
{
    [DependsOn(
        typeof(ReportsCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ReportsApplicationModule : AbpModule
    {
        #region Overrides of AbpModule

        /// <inheritdoc />
        public override void PreInitialize()
        {
            base.PreInitialize();
            Configuration.Authorization.Providers.Add<ReportsAuthorizationProvider>();
        }

        #endregion

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}