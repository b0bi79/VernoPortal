using System;
using System.Reflection;
using Abp.Modules;
using Abp.TestBase;

namespace Verno.Reports.Core.Tests
{
    [DependsOn(
         typeof(AbpTestBaseModule)
     )]
    public class ReportsCoreTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.

            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}