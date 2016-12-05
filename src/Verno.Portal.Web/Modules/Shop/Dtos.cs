using System;
using System.ComponentModel.DataAnnotations;

namespace Verno.Portal.Web.Modules.Shop
{
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
        public string ImahPr { get; set; } // Производитель
        public decimal RealizSht { get; set; } // Реализация за предыдуший день (автомат) Данные за день пред недели
        public decimal SpisSht { get; set; } // Списания за предыдущий день (автомат)

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

        public int ToBake => RealizSht > 0 ? (int)Math.Round(RealizSht + (RealizSht * Koeff) / 100) : Normativ; // План на сегодня 
    }
}