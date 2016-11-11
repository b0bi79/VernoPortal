using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Verno.Reports.Web.Modules.Returns
{
    public abstract class ReturnsRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<ReturnsDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected ReturnsRepositoryBase(IDbContextProvider<ReturnsDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class ReturnsRepositoryBase<TEntity> : ReturnsRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected ReturnsRepositoryBase(IDbContextProvider<ReturnsDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }

    public class ReturnsRepository : ReturnsRepositoryBase<Return>
    {
        public ReturnsRepository(IDbContextProvider<ReturnsDbContext> dbContextProvider) : base(dbContextProvider) { }

        public async Task<Return> GetByRasxod(int rasxod)
        {
            return await GetAll().FirstOrDefaultAsync(r => r.Rasxod == rasxod);
        }
    }

    public class ReturnFilesRepository : ReturnsRepositoryBase<ReturnFile>
    {
        public ReturnFilesRepository(IDbContextProvider<ReturnsDbContext> dbContextProvider) : base(dbContextProvider) { }

        #region Overrides of EfCoreRepositoryBase<ReturnsDbContext,ReturnFile,int>

        /// <inheritdoc />
        public override IQueryable<ReturnFile> GetAll()
        {
            return base.GetAll().Where(r => !r.Deleted);
        }
        
        /// <inheritdoc />
        public override IQueryable<ReturnFile> GetAllIncluding(params Expression<Func<ReturnFile, object>>[] propertySelectors)
        {
            return base.GetAllIncluding(propertySelectors).Where(r => !r.Deleted);
        }

        public override void Delete(ReturnFile entity)
        {
            AttachIfNot(entity);
            //Table.Remove(entity);

            entity.Deleted = true;
        }

        #endregion

        public IQueryable<ReturnFile> GetByRasxod(int rasxod)
        {
            return GetAll().Where(r => r.Return.Rasxod == rasxod);
        }
    }
}