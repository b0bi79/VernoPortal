using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Verno.Identity.Data;

namespace Verno.Identity.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly HostRoleAndUserCreator _roleAndUserCreator;
        private readonly DefaultSettingsCreator _settingsCreator;

        /// <inheritdoc />
        public InitialHostDbBuilder(HostRoleAndUserCreator roleAndUserCreator, DefaultSettingsCreator settingsCreator)
        {
            _roleAndUserCreator = roleAndUserCreator;
            _settingsCreator = settingsCreator;
        }

        public void Build()
        {
            //context.Database.Migrate();
            //if (context.AllMigrationsApplied())
            {
                _roleAndUserCreator.Create();
                _settingsCreator.Create();
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
