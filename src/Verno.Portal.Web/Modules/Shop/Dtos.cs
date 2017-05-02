using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.AutoMapper;
using Verno.ShInfo.Models.Dto;

namespace Verno.Portal.Web.Modules.Shop
{
    [AutoMap(typeof(ProizvMagSpec))]
    public class ProizvMagSpecDto
    {
        public int Id { get; set; }
        public int Proekt { get; set; }
        public string ProektName { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool Promo { get; set; }

        public IEnumerable<ProizvMagSpecItemDto> Items { get; set; } = new List<ProizvMagSpecItemDto>();

        public override string ToString()
        {
            return $"[{Id}] {ProektName}: {DateFrom:d}-{DateTo:d} {(Promo ? "Promo" : "")}";
        }
    }

    [AutoMap(typeof(ProizvMagSpecItem))]
    public class ProizvMagSpecItemDto
    {
        public int Id { get; set; }
        public NomenklaturaDto Tovar { get; set; }
        public int? Norm { get; set; }
        public int? Koeff { get; set; }
    }

    public class ProductionCalc
    {
        private int? _normativ;
        private int? _koeff;
        private const int NormDef = 4;
        private const int KoeffDef = 10;

        [Key]
        public int VidTovara { get; set; } // Код св/у
        public string ImahKod2 { get; set; } // Подгруппа классификатор, 
        public string ShtrixKod { get; set; } // Код касса, 
        public string Naimenovanie { get; set; } // Наименование
        public string Etiketka { get; set; } // Etiketka
        public decimal RealizSht { get; set; } // Реализация за предыдуший день (автомат) Данные за день пред недели
        public decimal SpisSht { get; set; } // Списания за предыдущий день (автомат)
        public decimal OstSht { get; set; } // Остаток
        public int PlanVyp { get; set; } // План на сегодня

        // Норматив продаж (должна быть возможность менять из офиса)
        public int Normativ 
        {
            get { return _normativ ?? NormDef; }
            set { _normativ = value; }
        }

        // Коэффициент вносит торговля (должна быть возможность менять из офиса)
        public int Koeff 
        {
            get { return _koeff ?? KoeffDef; }
            set { _koeff = value; }
        }

        public int ToBake => PlanVyp; //RealizSht > 0 ? (int)Math.Round(RealizSht + (RealizSht * Koeff) / 100) : Normativ; // План на сегодня 
    }
}