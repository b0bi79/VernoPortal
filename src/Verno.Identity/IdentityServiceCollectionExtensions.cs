using System;
using Abp.AspNetCore;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Verno.Identity.Users;
using UserStore = Verno.Identity.Users.UserStore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Verno.Identity.Roles;

namespace Verno.Identity
{
    public static class IdentityServiceCollectionExtensions
    {
        public static void ConfigureIdentity(this AbpServiceOptions options, string applicationName)
        {
            options.IocManager.IocContainer.Register(
                Component.For<IRoleStore<Role>>()
                    .ImplementedBy<RoleStore<Role, Data.IdentityDbContext, int>>().LifestyleTransient()
                    /*.LifestylePerWebRequest()*/);
            options.IocManager.Register<UserManager<User>>(DependencyLifeStyle.Transient);
            options.IocManager.Register<IRepository<UserLoginAttempt, int>, UserLoginAttemptRepository>(DependencyLifeStyle.Transient);
            options.IocManager.IocContainer.Register(Component.For<IUserStore<User>>().ImplementedBy<UserStore>()
                .DependsOn(Parameter.ForKey("application").Eq(applicationName)).LifestyleTransient()/*.LifestyleSingleton()*//*.LifestylePerWebRequest()*/);
            options.IocManager.IocContainer.Register(Component.For<SignInManager>()
                .DependsOn(Parameter.ForKey("application").Eq(applicationName)).LifestyleTransient()/*.LifestyleSingleton()*//*.LifestylePerWebRequest()*/);
        }

        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddEntityFrameworkStores(this IdentityBuilder builder)
        {
            builder.Services.TryAdd(GetDefaultServices());
            return builder;
        }

        private static IServiceCollection GetDefaultServices()
        {
            var services = new ServiceCollection();
            services.AddScoped<IUserStore<User>, UserStore>();
            services.AddScoped<IRoleStore<Role>, RoleStore<Role, Data.IdentityDbContext, int>>();
            services.AddScoped<IRepository<UserLoginAttempt, int>, UserLoginAttemptRepository>();
            return services;
        }
    }
}