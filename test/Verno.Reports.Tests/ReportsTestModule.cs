using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Modules;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using Abp.TestBase;
using Verno.Reports.EntityFrameworkCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Verno.Identity;
using Verno.Identity.Authorization;
using Verno.Identity.Data;
using Verno.Identity.Migrations.SeedData;
using Verno.Identity.Roles;
using Verno.Identity.Users;
using Verno.Reports.Authorization;
using Verno.Reports.Tests.TestDatas;

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

            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            SetupInMemoryDb();
            SetupIdentity();
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

        private void SetupIdentity()
        {
            var context = Substitute.For<HttpContext>();

            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Returns(context);

            IocManager.IocContainer.Register(Component.For<IHttpContextAccessor>().Instance(contextAccessor).LifestyleSingleton());
            //IocManager.IocContainer.Register(Component.For<IPermissionChecker>().Instance(Substitute.For<IPermissionChecker>()).LifestyleSingleton());

            IocManager.Register<TestIdentityBuilder>();
            IocManager.Register<InitialHostDbBuilder>();
            IocManager.Register<HostRoleAndUserCreator> ();
            IocManager.Register<DefaultSettingsCreator>();
            IocManager.IocContainer.Register(Component.For<AuthorizationProvider>().ImplementedBy<IdentityAuthorizationProvider>().Named(Guid.NewGuid().ToString()).IsFallback());
            IocManager.IocContainer.Register(Component.For<AuthorizationProvider>().ImplementedBy<ReportsAuthorizationProvider>().Named(Guid.NewGuid().ToString()).IsFallback());
            IocManager.Register<IdentityDbContext>(DependencyLifeStyle.Transient);
            IocManager.Register<IHttpContextAccessor, HttpContextAccessor>();
            IocManager.Register<IdentityMarkerService>(DependencyLifeStyle.Transient);
            IocManager.Register<IUserValidator<User>, UserValidator<User>>(DependencyLifeStyle.Transient);
            IocManager.Register<IPasswordValidator<User>, PasswordValidator<User>>(DependencyLifeStyle.Transient);
            IocManager.Register<IPasswordHasher<User>, PasswordHasher<User>>(DependencyLifeStyle.Transient);
            IocManager.Register<ILookupNormalizer, UpperInvariantLookupNormalizer>(DependencyLifeStyle.Transient);
            IocManager.Register<IRoleValidator<Role>, RoleValidator<Role>>(DependencyLifeStyle.Transient);
            IocManager.Register<IdentityErrorDescriber>(DependencyLifeStyle.Transient);
            IocManager.Register<ISecurityStampValidator, SecurityStampValidator<User>>(DependencyLifeStyle.Transient);
            IocManager.Register<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User, Role>>(DependencyLifeStyle.Transient);
            IocManager.IocContainer.Register(Component.For<UserManager>()
                                      .ImplementedBy<FakeUserManager>()
                                      .OverridesExistingRegistration()
                                      .LifestyleTransient());
            IocManager.IocContainer.Register(Component.For<RoleManager>()
                                      .ImplementedBy<FakeRoleManager>()
                                      .OverridesExistingRegistration()
                                      .LifestyleTransient());
            //IocManager.Register<SignInManager<User>, SignInManager>(DependencyLifeStyle.Transient);
            //IocManager.Register<RoleManager<Role>, RoleManager>(DependencyLifeStyle.Transient);
        }
    }

    public class FakeUserManager : UserManager
    {
        public FakeUserManager(IUserStore<User> userStore, IPasswordHasher<User> passwordHasher, IPermissionManager permissionManager,
            ICacheManager cacheManager, RoleManager roleManager)
            : base(userStore,
                  Substitute.For<IOptions<IdentityOptions>>(),
                  passwordHasher,
                  new [] {Substitute.For<IUserValidator<User>>()},
                  new[] {Substitute.For<IPasswordValidator<User>>()},
                  Substitute.For<ILookupNormalizer>(),
                  Substitute.For<IdentityErrorDescriber>(),
                  Substitute.For<IServiceProvider>(),
                  Substitute.For<ILogger<UserManager<User>>>(),
                  permissionManager,
                  cacheManager,
                  roleManager)
        { }

        /*private static IPermissionManager CreatePermissionManager()
        {
            var pm = Substitute.For<IPermissionManager>();
            pm.GetPermission(Arg.Any<string>()).Returns(x => new Permission(x.Arg<string>()));
            return pm;
        }*/

        public override Task<User> FindByEmailAsync(string email)
        {
            return Task.FromResult(new User { Email = email });
        }

        public override Task<bool> IsEmailConfirmedAsync(User user)
        {
            return Task.FromResult(user.Email == "test@test.com");
        }

        public override Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return Task.FromResult("---------------");
        }
    }

    public class FakeRoleManager : RoleManager
    {
        // IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger, IHttpContextAccessor contextAccessor, IPermissionManager permissionManager, ICacheManager cacheManager
        /// <inheritdoc />
        public FakeRoleManager(IRoleStore<Role> store, ILookupNormalizer keyNormalizer, IHttpContextAccessor contextAccessor, IPermissionManager permissionManager, ICacheManager cacheManager) 
            : base(store,
                  new[] { Substitute.For<IRoleValidator<Role>>() },
                  keyNormalizer,
                  Substitute.For<IdentityErrorDescriber>(),
                  Substitute.For<ILogger<RoleManager>>(), 
                  contextAccessor, 
                  permissionManager, 
                  cacheManager)
        {
        }
    }
}