using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.Reports.Models
{
    [Table("Reports_OutFormats")]
    public class ReportOutFormat : Entity<int>
    {
        public ReportOutFormat()
        {
        }

        public ReportOutFormat(string outFormatId, string template = null, string displayText = null)
        {
            OutFormatId = outFormatId;
            DisplayText = displayText;
            Template = template;
        }

        public int ReportId { get; set; }
        public string OutFormatId { get; set; }
        public string DisplayText { get; set; }
        public string Template { get; set; }

        public Report Report { get; set; }
        public OutFormat OutFormat { get; set; }
    }
}