using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Verno.ShInfo.Models;

namespace Verno.Portal.Web.Modules.Shop
{
    public class ShopDbContext : AbpDbContext
    {
        public DbSet<ProductionCalc> ProductionCalcs { get; set; }
        public DbSet<Nomenklatura> ProductionList { get; set; }
        public DbSet<KonfProizvMag> KonfProizvMag { get; set; }
        public DbSet<ProizvMagSpec> ProizvMagSpecs { get; set; }
        public DbSet<ProizvMagSpecItem> ProizvMagSpecItems { get; set; }

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
            Database.SetCommandTimeout(120);
            var proc = @"
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
      OstSht MONEY NULL DEFAULT(0),
      Normativ INT NULL,
      Koeff INT NULL,
      PlanVyp INT NULL,    
      Etiketka NVARCHAR(1000) NULL
)
CREATE INDEX IND1 ON #Matr (VidTovara,NomerSklada)
 
exec shReportsSQL.dbo.KalkProizvMagNew {0}

select k2.ImahKod2, #Matr.VidTovara, ShtrixKod, Naimenovanie, RealizSht, SpisSht, OstSht,
    Normativ, Koeff, PlanVyp, Etiketka
from #Matr
inner join shInfoSQL.dbo.Kod2 k2 on #Matr.Kod1=k2.Kod1 and #Matr.Kod2=k2.kod2
LEFT JOIN shInfoSQL.dbo.PereKod AS Pe WITH(NOLOCK) ON #Matr.VidTovara=Pe.NovKod
INNER JOIN shInfoSQL.dbo.ShtrixKody sk on isnull(Pe.StarKod,#Matr.VidTovara)=sk.VidTovara and len(ShtrixKod)<=4";
            return ProductionCalcs.FromSql(proc, shopNum)
                .ToListAsync();
        }

        public Task<List<Nomenklatura>> GetProductionTovars(int proekt)
        {
            Database.SetCommandTimeout(120);
            var proc = @"
SELECT Nm.VidTovara Id, Nm.Naimenovanie, Nm.KolichestvoUpakovka, Nm.EdinicyIzmereniah
FROM shInfoSQL.dbo.Nomenklatura AS Nm WITH(NOLOCK)
	INNER JOIN shInfoSQL.dbo.ProektyTovarov AS PTv WITH(NOLOCK) ON Nm.VidTovara = PTv.VidTovara
	LEFT JOIN shInfoSQL.dbo.PereKod AS Pk WITH(NOLOCK) ON Nm.VidTovara = Pk.StarKod
WHERE (PTv.Proekt = {0}) AND (Nm.VidTovara > 3) AND (Nm.Tara = 0) AND (PTv.Matr <> 0) and (PTv.ZaprZakaz=0)
	AND (Nm.Kod1 IN (25)) AND (Nm.Kod2 IN (4)) and ISNULL(Pk.NovKod,Nm.VidTovara)=Nm.VidTovara";
            return ProductionList.FromSql(proc, proekt)
                .ToListAsync();
        }
    }
}
