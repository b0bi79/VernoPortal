using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.Reports.Models
{
    [Table("ReportColumns")]
    public class ReportColumn : Entity<int>
    {
        public ReportColumn()
        {
        }

        public ReportColumn(string field, string title, string format = null, string linkUrl = null, string formula = null, 
            string aggregate = null, bool display = true, int sortOrder = 0, int? columnGroupId = null, int tableNo = 0)
        {
            Aggregate = aggregate;
            ColumnGroupId = columnGroupId;
            TableNo = tableNo;
            Display = display;
            Field = field;
            Format = format;
            Formula = formula;
            LinkUrl = linkUrl;
            SortOrder = sortOrder;
            Title = title;
        }

        public int TableNo { get; set; }
        public string Field { get; set; }
        public string Title { get; set; }
        public string Format { get; set; }
        public string LinkUrl { get; set; }
        public string Formula { get; set; }
        public string Aggregate { get; set; }
        public bool Display { get; set; }
        public int SortOrder { get; set; }
        public int? ColumnGroupId { get; set; }

        public int ReportId { get; set; }
        public Report Report { get; set; }
    }

}