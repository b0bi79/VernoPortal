using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Verno.Reports.Web.Modules.Returns
{
    public class ReturnsDbContext : AbpDbContext
    {
        public DbSet<ReturnData> ReturnDatas { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<ReturnFile> ReturnFiles { get; set; }

        public ReturnsDbContext(DbContextOptions<ReturnsDbContext> options)
            : base(options)
        {
        }

        /*protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // This needs to go before the other rules!
            builder.Entity<ReturnData>(b =>
            {
                b.Property(u => u.DocDate).HasColumnType("datetime");
            });
            builder.Entity<ReturnFile>(b =>
            {
                b.Property(u => u.DateLot).HasColumnType("datetime");
                b.Property(u => u.EditDate).HasColumnType("datetime");
            });
        }*/
    }
}
