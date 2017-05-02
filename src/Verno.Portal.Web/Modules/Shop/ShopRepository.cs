using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Verno.Portal.Web.Modules.Shop
{
    public abstract class ShopRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<ShopDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected ShopRepositoryBase(IDbContextProvider<ShopDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class ShopRepositoryBase<TEntity> : ShopRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected ShopRepositoryBase(IDbContextProvider<ShopDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }

    public class ProizvMagSpecRepository : ShopRepositoryBase<ProizvMagSpec>
    {
        public ProizvMagSpecRepository(IDbContextProvider<ShopDbContext> dbContextProvider) : base(dbContextProvider) { }
    }

    public class ProizvMagSpecItemRepository : ShopRepositoryBase<ProizvMagSpecItem>
    {
        public ProizvMagSpecItemRepository(IDbContextProvider<ShopDbContext> dbContextProvider) : base(dbContextProvider) { }

        public IQueryable<ProizvMagSpecItem> GetForSpec(int specId)
        {
            return GetAll().Include(i=>i.Tovar).Where(i => i.ProizvMagSpecId == specId);
        }
    }
}