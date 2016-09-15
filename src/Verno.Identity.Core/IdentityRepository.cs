using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Verno.Identity.Data;

namespace Verno.Identity
{
    public class IdentityRepository<TEntity, TPrimaryKey> : EfCoreRepositoryBase<IdentityDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public IdentityRepository(IDbContextProvider<IdentityDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}