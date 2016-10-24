using Microsoft.EntityFrameworkCore;

namespace Verno.Reports.EntityFrameworkCore
{
    public static class DbContextOptionsConfigurer
    {
        public static void Configure(
            DbContextOptionsBuilder<ReportsDbContext> dbContextOptions,
            string connectionString
            )
        {
            /* This is the single point to configure DbContextOptions for ReportsDbContext */
            dbContextOptions.UseSqlServer(connectionString);
        }
    }
}