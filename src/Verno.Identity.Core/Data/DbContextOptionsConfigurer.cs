using Microsoft.EntityFrameworkCore;

namespace Verno.Identity.Data
{
    public static class DbContextOptionsConfigurer
    {
        public static void Configure(
            DbContextOptionsBuilder<IdentityDbContext> dbContextOptions,
            string connectionString
            )
        {
            /* This is the single point to configure DbContextOptions for ReportsDbContext */
            dbContextOptions.UseSqlServer(connectionString);
        }
    }
}