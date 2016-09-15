using System;

namespace Verno.Reports.Web.Modules.Print
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

        public PrintDto(int? liniah, string nomNakl, DateTime? dataNakl, string imahDok, int? srcWhId, string srcWarehouse, string url)
        {
            DataNakl = dataNakl;
            ImahDok = imahDok;
            Liniah = liniah;
            NomNakl = nomNakl;
            Url = url;
            SrcWhId = srcWhId;
            SrcWarehouse = srcWarehouse;
        }
    }
}