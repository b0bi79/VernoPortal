using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Verno.Identity.Data;

namespace Verno.Identity.Migrations.SeedData
{
    public static class InitialHostDbBuilder
    {
        public static void Build(this IdentityDbContext context)
        {
            //context.Database.Migrate();
            //if (context.AllMigrationsApplied())
            {
                new HostRoleAndUserCreator(context).Create();
                new DefaultSettingsCreator(context).Create();
            }
        }
    }

    public static class DbContextExtensions
    {
        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }
    }
}
