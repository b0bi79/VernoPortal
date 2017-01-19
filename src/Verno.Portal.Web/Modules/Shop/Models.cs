using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Verno.Portal.Web.Modules.Shop
{
    public class KonfProizvMag : Entity<int>
    {
        // PRIMARY KEY CNTaa
        public int VidTovara { get; set; }
        public int? Normativ { get; set; }
        public int? Koeff { get; set; }
        public string Etiketka { get; set; }
    }

    /*[Table("Dokumenty")]
    public class PrintDoc : Entity<int>
    {
        // PRIMARY KEY DokId
        public int? SklIst { get; set; }
        public int? MagIst { get; set; }
        public int? SklPol { get; set; }
        public int? MagPol { get; set; }
        public int? Liniah { get; set; }
        public string NomNakl { get; set; }
        public DateTime? DataNakl { get; set; }

        [ForeignKey("DokId")]
        public virtual ICollection<PrintDocForm> Forms { get; set; }
    }*/
}