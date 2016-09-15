using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Verno.Identity.Data
{
    /* This class is needed to run "dotnet ef ..." commands from command line.
     */
    public class ReportsDbContextFactory : IDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<IdentityDbContext>();
            builder.UseSqlServer("Server=localhost; Database=UserIdentity; Trusted_Connection=True;");
            return new IdentityDbContext(builder.Options);
        }
    }
}