using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;

namespace Verno.ShInfo.EntityFrameworkCore.Repositories
{
    public abstract class ShInfoRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<ShInfoDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected ShInfoRepositoryBase(IDbContextProvider<ShInfoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class ShInfoRepositoryBase<TEntity> : ShInfoRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected ShInfoRepositoryBase(IDbContextProvider<ShInfoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}