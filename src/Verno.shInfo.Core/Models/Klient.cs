using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.ShInfo.Models
{
    [Table("Klienty")]
    public class Klient: Entity<int>
    {
        // PRIMARY KEY NomerKlienta 
        public int Filial { get; set; }
        public DateTime Data { get; set; }
        public int KodVladelqca { get; set; }
        public string Kod { get; set; }
        public string Kategoriah { get; set; }
        public string Nazvanie { get; set; }
        public string Adres { get; set; }
        public string Predstavitelq { get; set; }
        public string Telefon { get; set; }
        public string Faks { get; set; }
        public string BankovskieRekvizity { get; set; }
        public string OKPO { get; set; }
        public string OKONX { get; set; }
        public string INN { get; set; }
        public string Bank { get; set; }
        public string RaschSch { get; set; }
        public string KorrSch { get; set; }
        public string BIK { get; set; }
        public int My { get; set; }
        public string Primechaniah { get; set; }
        public int Liniah { get; set; }
        public int PrajsList { get; set; }
        public string NazvOfic { get; set; }
        public string AdresOfic { get; set; }
        public string KPPKlient { get; set; }
        public string KPPFakt { get; set; }
        public int KodPostavthik { get; set; }
        public int NalogPrDop { get; set; }
        public string EgaisRegId { get; set; }
        public decimal PlanRTO { get; set; }

        public ZaahvKlient ZaahvKlient { get; set; }
    }
}