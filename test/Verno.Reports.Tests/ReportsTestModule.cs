using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.TestBase;
using Verno.Reports.EntityFrameworkCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Verno.Identity;
using Verno.Identity.Data;

namespace Verno.Reports.Tests
{
    [DependsOn(
        typeof(IdentityModule),
        typeof(ReportsApplicationModule),
        typeof(ReportsEntityFrameworkCoreModule),
        typeof(AbpTestBaseModule)
        )]
    public class ReportsTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
            SetupInMemoryDb();

            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        private void SetupInMemoryDb()
        {
            var services = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );

            var builder = new DbContextOptionsBuilder<ReportsDbContext>();
            builder.UseInMemoryDatabase().UseInternalServiceProvider(serviceProvider);

            var identityBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            identityBuilder.UseInMemoryDatabase().UseInternalServiceProvider(serviceProvider);

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<ReportsDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );
            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<IdentityDbContext>>()
                    .Instance(identityBuilder.Options)
                    .LifestyleSingleton()
            );
        }
    }
}