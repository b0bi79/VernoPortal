using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Verno.Portal.Web.Models;

namespace Verno.Portal.Web.DataAccess
{
    public class ShPrintSqlDbContext : AbpDbContext
    {
        public DbSet<OperationsLog> OperationsLog { get; set; }

        public ShPrintSqlDbContext(DbContextOptions<ShPrintSqlDbContext> options)
            : base(options)
        {
        }
    }

    public abstract class ShPrintSqlRepositoryBase<TEntity> : EfCoreRepositoryBase<ShPrintSqlDbContext, TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected ShPrintSqlRepositoryBase(IDbContextProvider<ShPrintSqlDbContext> dbContextProvider)
            : base(dbContextProvider)
        { }
    }

    public class OperationsLogRepository : ShPrintSqlRepositoryBase<OperationsLog>
    {
        public OperationsLogRepository(IDbContextProvider<ShPrintSqlDbContext> dbContextProvider) : base(dbContextProvider) { }
    }
}
