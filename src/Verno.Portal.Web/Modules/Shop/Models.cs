using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Verno.ShInfo.Models;

namespace Verno.Portal.Web.Modules.Shop
{
    public class KonfProizvMag : Entity<int>
    {
        // PRIMARY KEY CNTaa
        public int VidTovara { get; set; }
        public int? Normativ { get; set; }
        public int? Koeff { get; set; }
        public string Etiketka { get; set; }

        public int? Filial { get; set; }
        public string Areal { get; set; }
        public int? Format { get; set; }
    }

    [Table("ProizvMagSpec")]
    public class ProizvMagSpec : Entity<int>
    {
        public int Proekt { get; set; }
        [Column("Data")]
        public DateTime Date { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool Promo { get; set; }

        public List<ProizvMagSpecItem> Items { get; set; }
    }

    public class ProizvMagSpecItem : Entity<int>
    {
        public int ProizvMagSpecId { get; set; }
        public int VidTovara { get; set; }
        public int? Norm { get; set; }
        public int? Koeff { get; set; }

        [ForeignKey("ProizvMagSpecId")]
        public ProizvMagSpec ProizvMagSpec { get; set; }

        [ForeignKey("VidTovara")]
        public Nomenklatura Tovar { get; set; }
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