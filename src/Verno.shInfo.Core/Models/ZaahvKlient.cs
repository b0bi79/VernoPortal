using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.ShInfo.Models
{
    public class ZaahvKlient : Entity<int>
    {
        // PRIMARY KEY CntKlZaah 
        public int NomerKlienta { get; set; }
        public short IstId { get; set; }
        public int NomerSklada { get; set; }
        public int Ind { get; set; }
        public int? ZakazRez { get; set; }
        public int Fmt { get; set; }
        public int PrajsRzh { get; set; }
        public short Sxema { get; set; }
        public int? KNZ { get; set; }
        public int MyTip1 { get; set; }
        public int MyTip2 { get; set; }
        public int Proekt { get; set; }
        public int Prajs { get; set; }
        public int IndSp { get; set; }
        public DateTime VremahSp { get; set; }

        [ForeignKey("NomerKlienta")]
        public Klient Klient { get; set; }
    }
}