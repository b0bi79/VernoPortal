using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;

namespace Verno.Reports.EntityFrameworkCore.Repositories
{
    public abstract class ReportsRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<ReportsDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected ReportsRepositoryBase(IDbContextProvider<ReportsDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class ReportsRepositoryBase<TEntity> : ReportsRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected ReportsRepositoryBase(IDbContextProvider<ReportsDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}