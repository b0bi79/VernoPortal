using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Verno.ShInfo.Models;

namespace Verno.ShInfo.EntityFrameworkCore
{
    public class ShInfoDbContext : AbpDbContext
    {
        public DbSet<Nomenklatura> Nomenklatura { get; set; }
        public DbSet<ZaahvKlient> ZaahvKlient { get; set; }
        public DbSet<Klient> Klienty { get; set; }
        public DbSet<Proekt> Proekts { get; set; }
        public DbSet<Filial> Filials { get; set; }

        public ShInfoDbContext(DbContextOptions<ShInfoDbContext> options) 
            : base(options)
        {

        }

        #region Overrides of AbpDbContext

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Nomenklatura>().Property(u => u.Id).HasColumnName("VidTovara");
            builder.Entity<Klient>().Property(u => u.Id).HasColumnName("NomerKlienta");
            builder.Entity<ZaahvKlient>().Property(u => u.Id).HasColumnName("CntKlZaah");
            builder.Entity<Proekt>().Property(u => u.Id).HasColumnName("Proekt");
            builder.Entity<Filial>().Property(u => u.Id).HasColumnName("Filial");
        }

        #endregion
    }
}
