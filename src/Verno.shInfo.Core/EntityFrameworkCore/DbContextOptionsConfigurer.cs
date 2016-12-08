using Microsoft.EntityFrameworkCore;

namespace Verno.ShInfo.EntityFrameworkCore
{
    public static class DbContextOptionsConfigurer
    {
        public static void Configure(
            DbContextOptionsBuilder<ShInfoDbContext> dbContextOptions,
            string connectionString
            )
        {
            /* This is the single point to configure DbContextOptions for ReportsDbContext */
            dbContextOptions.UseSqlServer(connectionString);
        }
    }
}