using Abp.AspNetCore;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Session;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Identity;
using Verno.Identity.Data;
using Verno.Identity.Organizations;
using Verno.Identity.Roles;
using Verno.Identity.Settings;
using Verno.Identity.Users;

namespace Verno.Identity
{
    public static class IdentityServiceCollectionExtensions
    {
        public static void ConfigureIdentity(this AbpServiceOptions options, string applicationName)
        {
            IdentityModule.ApplicationName = applicationName;
            options.IocManager.IocContainer.Register(Component.For<IUserStore<User>>().ImplementedBy<UserStore>()
                .DependsOn(Parameter.ForKey("application").Eq(applicationName)).LifestyleTransient());
            options.IocManager.IocContainer.Register(Component.For<SignInManager>()
                .DependsOn(Parameter.ForKey("application").Eq(applicationName)).LifestyleTransient());

            options.IocManager.IocContainer.Register(Component.For<IRoleStore<Role>>().ImplementedBy<RoleStore>().LifestyleTransient());
            options.IocManager.Register<UserManager<User>, UserManager>(DependencyLifeStyle.Transient);
            options.IocManager.Register<RoleManager<Role>, RoleManager>(DependencyLifeStyle.Transient);
            options.IocManager.Register<ISettingStore, SettingStore>(DependencyLifeStyle.Transient);
            options.IocManager.Register<IRepository<UserLoginAttempt, int>, IdentityRepository<UserLoginAttempt, int>>(DependencyLifeStyle.Transient);
            options.IocManager.Register<IRepository<Setting, int>, IdentityRepository<Setting, int>>(DependencyLifeStyle.Transient);
            options.IocManager.Register<IRepository<OrgUnit, int>, IdentityRepository<OrgUnit, int>>(DependencyLifeStyle.Transient);
            options.IocManager.Register<IRepository<RolePermissionSetting, int>, RolePermissionSettingRepository/*IdentityRepository<RolePermissionSetting, int>*/>(DependencyLifeStyle.Transient);
            options.IocManager.Register<IAbpSession, HttpContextAbpSession>();
        }
    }

    public class RolePermissionSettingRepository : EfCoreRepositoryBase<IdentityDbContext, RolePermissionSetting, int>
    {
        public RolePermissionSettingRepository(IDbContextProvider<IdentityDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}