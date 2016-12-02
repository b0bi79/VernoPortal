using System.Reflection;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Runtime.Session;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Identity;
using Verno.Identity.Data;
using Verno.Identity.Localization;
using Verno.Identity.Organizations;
using Verno.Identity.Roles;
using Verno.Identity.Settings;
using Verno.Identity.Users;

namespace Verno.Identity
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    public class IdentityModule : AbpModule
    {
        public static string ApplicationName;

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = IdentityConsts.ConnectionStringName;
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            IdentityLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void Initialize()
        {
            IocManager.IocContainer.Register(Component.For<IUserStore<User>>().ImplementedBy<UserStore>()
                .DependsOn(Parameter.ForKey("application").Eq(ApplicationName)).LifestyleTransient());
            IocManager.IocContainer.Register(Component.For<SignInManager>()
                .DependsOn(Parameter.ForKey("application").Eq(ApplicationName)).LifestyleTransient());

            IocManager.Register<IRoleStore<Role>, RoleStore>(DependencyLifeStyle.Transient);
            IocManager.Register<UserManager<User>, UserManager>(DependencyLifeStyle.Transient);
            IocManager.Register<RoleManager<Role>, RoleManager>(DependencyLifeStyle.Transient);
            IocManager.Register<ISettingStore, SettingStore>(DependencyLifeStyle.Transient);
            IocManager.Register<IRepository<UserLoginAttempt, int>, IdentityRepository<UserLoginAttempt, int>>(DependencyLifeStyle.Transient);
            IocManager.Register<IRepository<Setting, int>, IdentityRepository<Setting, int>>(DependencyLifeStyle.Transient);
            IocManager.Register<IRepository<OrgUnit, int>, IdentityRepository<OrgUnit, int>>(DependencyLifeStyle.Transient);
            IocManager.Register<IRepository<RolePermissionSetting, int>, RolePermissionSettingRepository/*IdentityRepository<RolePermissionSetting, int>*/>(DependencyLifeStyle.Transient);
            IocManager.Register<IAbpSession, HttpContextAbpSession>();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}