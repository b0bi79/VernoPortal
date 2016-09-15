using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Verno.Reports.Web.Modules.Returns
{
    public class ReturnsDbContext : AbpDbContext
    {
        public DbSet<PrintDoc> PrintDocs { get; set; }
        public DbSet<PrintDocForm> PrintDocForms { get; set; }
        public DbSet<PutqServ> PutqServ { get; set; }
        public DbSet<Sklad> Sklads { get; set; }

        public ReturnsDbContext(DbContextOptions<ReturnsDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // This needs to go before the other rules!
            builder.Entity<PrintDoc>(b =>
            {
                b.HasKey(u => u.Id);
                b.ToTable("Dokumenty");
                b.HasMany(u => u.Forms).WithOne();
                b.Property(u => u.Id).HasColumnName("DokId");
                b.Property(u => u.DataNakl).HasColumnType("datetime");
                b.HasOne(u => u.SkladSrc).WithMany().HasForeignKey(p => p.SklIst).HasPrincipalKey(p => p.NomerSklada);
            });
            builder.Entity<PrintDocForm>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).HasColumnName("FrmId");
            });
            builder.Entity<Sklad>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).HasColumnName("Sklad");
            });
            builder.Entity<PutqServ>().HasKey(u => u.Id).HasName("CntAA");
        }
    }
}
