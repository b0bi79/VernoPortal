using System;
using System.Threading.Tasks;
using Abp.TestBase;
using Verno.Reports.EntityFrameworkCore;
using Verno.Reports.Tests.TestDatas;

namespace Verno.Reports.Tests
{
    public class ReportsTestBase : AbpIntegratedTestBase<ReportsTestModule>
    {
        public ReportsTestBase()

        {
            UsingDbContext(context => new TestDataBuilder(context).Build());
        }

        protected virtual void UsingDbContext(Action<ReportsDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<ReportsDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected virtual T UsingDbContext<T>(Func<ReportsDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<ReportsDbContext>())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected virtual async Task UsingDbContextAsync(Func<ReportsDbContext, Task> action)
        {
            using (var context = LocalIocManager.Resolve<ReportsDbContext>())
            {
                await action(context);
                await context.SaveChangesAsync(true);
            }
        }

        protected virtual async Task<T> UsingDbContextAsync<T>(Func<ReportsDbContext, Task<T>> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<ReportsDbContext>())
            {
                result = await func(context);
                context.SaveChanges();
            }

            return result;
        }
    }
}
