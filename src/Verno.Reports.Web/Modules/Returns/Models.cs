using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.Reports.Web.Modules.Returns
{
    [Table("Dokumenty")]
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

        public virtual Sklad SkladSrc { get; set; }
    }

    [Table("FormyDokumentov")]
    public class PrintDocForm : Entity<int>
    {
        // PRIMARY KEY FrmId
        public int? DokId { get; set; }
        public int? TipDok { get; set; }
        public string ImahDok { get; set; }
        public string Putq { get; set; }
        public DateTime? DataPech { get; set; }
        public DateTime? Deleted { get; set; }
        public DateTime? Created { get; set; }
    }

    [Table("Sklady")]
    public class Sklad : Entity<int>
    {
        public int? NomerSklada { get; set; }
        public string Postavthik { get; set; }
        public int? Filial { get; set; }
        public string Platelqthik { get; set; }
        public short? FlagD { get; set; }
        public string Ruk { get; set; }
        public string GlBux { get; set; }
        public string PutqPochta { get; set; }
        public int? TipNakl { get; set; }
        public short? Ind { get; set; }
        public int? KodGr { get; set; }
        public string Adres { get; set; }
        public int? Brak { get; set; }
        public int? Opt { get; set; }
        public int? MySkl { get; set; }
        public short? Otkr { get; set; }
        public int? IndSp { get; set; }
        public DateTime? VremahSp { get; set; }
        public short? OtprSp { get; set; }
        public int? VidPodd { get; set; }
        public int? SvahzSkl { get; set; }
        public int? Fas { get; set; }
        public short? Lim { get; set; }
        public int? LimGB { get; set; }
        public int? PLnRegion { get; set; }
        public int? SubkontoSkl { get; set; }
        public string KPPSklad { get; set; }
        public string ServerPath { get; set; }
        public short? ZaprKorrNakl { get; set; }
        public string EgaisRegId { get; set; }
    }

    public class PutqServ : Entity<int>
    {
        public string Putq { get; set; }
    }
}