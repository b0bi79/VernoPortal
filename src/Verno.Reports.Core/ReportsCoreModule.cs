using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Verno.Reports.DataSource;
using Verno.Reports.Executing;
using Verno.Reports.Localization;
using Verno.Reports.Utils;

namespace Verno.Reports
{
    public class ReportsCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            ReportsLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.IocContainer.AddFacility<TypedFactoryFacility>();

            IocManager.Register<ReportGenerator, ImportFromSqlReportGenerator>("importfromsql", DependencyLifeStyle.Transient);
            IocManager.Register<ReportGenerator, XslTransformReportGenerator>("xsltransform", DependencyLifeStyle.Transient);
            IocManager.Register<ReportGenerator, JsonReportGenerator>("jsontransform", DependencyLifeStyle.Transient);
            IocManager.IocContainer.Register(Component.For<IReportGeneratorFactory>()
                .AsFactory(c => c.SelectedWith(new ReportGeneratorTypedFactoryComponentSelector())));

            IocManager.Register<IReportDataSource, SqlFileReportDataSource>("sqlfile", DependencyLifeStyle.Transient);
            IocManager.Register<IReportDataSource, SqlProcReportDataSource>("sqlproc", DependencyLifeStyle.Transient);
            IocManager.IocContainer.Register(Component.For<IReportDataSourceFactory>()
                .AsFactory(c => c.SelectedWith(new DataSourceTypedFactoryComponentSelector())));
        }
    }
}