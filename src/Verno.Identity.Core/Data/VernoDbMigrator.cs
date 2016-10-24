using System.Configuration;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Verno.Identity.Data
{
    /*public abstract class AbpZeroDbMigrator<TDbContext, TConfiguration> : IVernoDbMigrator, ITransientDependency
         where TDbContext : DbContext
         where TConfiguration : DbMigrationsConfiguration<TDbContext>, new()
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IConnectionStringResolver _connectionStringResolver;
        private readonly IIocResolver _iocResolver;

        protected AbpZeroDbMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IConnectionStringResolver connectionStringResolver,
            IIocResolver iocResolver)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _connectionStringResolver = connectionStringResolver;
            _iocResolver = iocResolver;
        }

        public virtual void CreateOrMigrate()
        {
            var args = new ConnectionStringResolveArgs(MultiTenancySides.Host);

            args["DbContextType"] = typeof(TDbContext);
            args["DbContextConcreteType"] = typeof(TDbContext);

            var nameOrConnectionString = GetConnectionString(
                _connectionStringResolver.GetNameOrConnectionString(args)
            );

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                using (var dbContext = _iocResolver.ResolveAsDisposable<TDbContext>(new { nameOrConnectionString = nameOrConnectionString }))
                {
                    var dbInitializer = new MigrateDatabaseToLatestVersion<TDbContext, TConfiguration>(
                        true,
                        new TConfiguration());

                    dbInitializer.InitializeDatabase(dbContext.Object);

                    _unitOfWorkManager.Current.SaveChanges();
                    uow.Complete();
                }
            }
        }

        public static string GetConnectionString(string nameOrConnectionString)
        {
            var connStrSection = ConfigurationManager.ConnectionStrings[nameOrConnectionString];
            if (connStrSection != null)
            {
                return connStrSection.ConnectionString;
            }

            return nameOrConnectionString;
        }
    }*/
}