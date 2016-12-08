using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Verno.Portal.Web.Modules.Shop
{
    public class ShopDbContext : AbpDbContext
    {
        public DbSet<ProductionCalc> ProductionCalcs { get; set; }
        public DbSet<KonfProizvMag> KonfProizvMag { get; set; }

        public ShopDbContext(DbContextOptions<ShopDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // This needs to go before the other rules!
            builder.Entity<KonfProizvMag>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).HasColumnName("CNTaa");
            });
        }

        public Task<List<ProductionCalc>> GetProductionCalculator(int shopNum)
        {
            return ProductionCalcs.FromSql(@"
if object_id('tempdb..#Matr')>0 DROP TABLE #Matr

CREATE TABLE #Matr (
      CntAA INT IDENTITY(1,1) PRIMARY KEY,
      VidTovara INT NULL,
      Naimenovanie VARCHAR(200) NULL,
      KodTovara VARCHAR(5) NULL,
      KodPost VARCHAR(5) NULL,
      NomPr INT NULL,
      Filial INT NULL,
      Proekt INT NULL,
      Kod1 INT NULL,
      Kod2 INT NULL,
      Fmt INT NULL,
      NomerSklada INT NULL,
      FmtMag INT NULL,
      RealizSht MONEY NULL DEFAULT(0),
      SpisSht MONEY NULL DEFAULT(0),
)
CREATE INDEX IND1 ON #Matr (VidTovara,NomerSklada)

exec shReportsSQL.dbo.KalkProizvMag {0} --magazin

select k2.ImahKod2, #Matr.VidTovara, ShtrixKod, Naimenovanie, ImahPr, RealizSht, SpisSht, kpm.Normativ, kpm.Koeff
from #Matr
	inner join shInfoSQL.dbo.Kod2 k2 on #Matr.Kod1=k2.Kod1 and #Matr.Kod2=k2.kod2
    left join shU1SQL.dbo.KonfProizvMag kpm on #matr.VidTovara=kpm.VidTovara --and #matr.Proekt=kpm.Proekt 
    left join shInfoSQL.dbo.ShtrixKody sk on #Matr.VidTovara=sk.VidTovara and len(ShtrixKod)<=4
	left join shInfoSQL.dbo.Proizvoditeli pr on #Matr.KodPost=pr.KodPost", shopNum)
                .ToListAsync();
        }
    }
}
