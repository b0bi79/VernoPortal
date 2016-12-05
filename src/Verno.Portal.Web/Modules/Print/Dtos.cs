using System;

namespace Verno.Portal.Web.Modules.Print
{
    public class PrintDto
    {
        public int? Liniah { get; set; }
        public string NomNakl { get; set; }
        public DateTime? DataNakl { get; set; }
        public string ImahDok { get; set; }
        public string Url { get; set; }
        public string SrcWarehouse { get; set; }
        public int? SrcWhId { get; set; }
        public int ShopNum { get; set; }

        public PrintDto(int? liniah, string nomNakl, int? shopNum, DateTime? dataNakl, string imahDok, int? srcWhId, string srcWarehouse, string url)
        {
            DataNakl = dataNakl;
            ImahDok = imahDok;
            Liniah = liniah;
            NomNakl = nomNakl;
            ShopNum = shopNum ?? 0;
            Url = url;
            SrcWhId = srcWhId;
            SrcWarehouse = srcWarehouse;
        }
    }
}