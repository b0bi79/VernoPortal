using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Verno.Reports.Products;

namespace Verno.Reports.EntityFrameworkCore
{
    public class ReportsDbContext : AbpDbContext
    {
        //TODO: Define an DbSet for each Entity...

        public DbSet<Product> Products { get; set; }

        public ReportsDbContext(DbContextOptions<ReportsDbContext> options) 
            : base(options)
        {

        }
    }
}
