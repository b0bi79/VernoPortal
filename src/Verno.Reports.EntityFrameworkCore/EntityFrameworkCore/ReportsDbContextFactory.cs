using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Verno.Reports.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class ReportsDbContextFactory : IDbContextFactory<ReportsDbContext>
    {
        public ReportsDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<ReportsDbContext>();
            builder.UseSqlServer("Server=localhost; Database=AbpProjectNameDb; Trusted_Connection=True;");
            return new ReportsDbContext(builder.Options);
        }
    }
}