using System;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Effort;
using Verno.Identity.Data;
using Verno.Identity.Users;
using Verno.Reports.EntityFrameworkCore;
using Verno.Reports.Tests.TestDatas;

namespace Verno.Reports.Tests
{
    public abstract class ReportsTestBase : AbpIntegratedTestBase<ReportsTestModule>
    {
        private DbConnection _hostDb;

        protected ReportsTestBase()
        {
            //Seed initial data for host
            LocalIocManager.Resolve<TestIdentityBuilder>().Build();
            UsingDbContext(context => new TestDataBuilder(context).Build());

            LoginAsHostAdmin();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            /* You can switch database architecture here: */
            UseSingleDatabase();
        }

        /* Uses single database for host. */
        private void UseSingleDatabase()
        {
            _hostDb = DbConnectionFactory.CreateTransient();

            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(() => _hostDb)
                    .LifestyleSingleton()
                );
        }

        #region UsingDbContext

        protected void UsingDbContext(Action<IdentityDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<IdentityDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected void UsingDbContext(Action<ReportsDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<ReportsDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected async Task UsingDbContextAsync(Func<IdentityDbContext, Task> action)
        {
                using (var context = LocalIocManager.Resolve<IdentityDbContext>())
                {
                    await action(context);
                    await context.SaveChangesAsync(true);
                }
        }

        protected T UsingDbContext<T>(Func<IdentityDbContext, T> func)
        {
            T result;

                using (var context = LocalIocManager.Resolve<IdentityDbContext>())
                {
                    result = func(context);
                    context.SaveChanges();
                }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(Func<IdentityDbContext, Task<T>> func)
        {
            T result;

                using (var context = LocalIocManager.Resolve<IdentityDbContext>())
                {
                    result = await func(context);
                    await context.SaveChangesAsync(true);
                }

            return result;
        }

        #endregion

        #region Login

        protected void LoginAsHostAdmin()
        {
            LoginAsHost(User.AdminUserName);
        }

        protected void LoginAsHost(string userName)
        {
            var user =
                UsingDbContext(
                    context =>
                        context.Users.Include(x=>x.Claims).FirstOrDefault(u => u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for host.");
            }

            AbpSession.UserId = user.Id;
            var claims = user.Claims.Select(x=>new Claim(x.ClaimType, x.ClaimValue)).ToList();
            claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, userName));
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        #endregion

        /// <summary>
        /// Gets current user if <see cref="IAbpSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = AbpSession.GetUserId();
            return await UsingDbContext(context => context.Users.SingleAsync(u => u.Id == userId));
        }
    }
}
