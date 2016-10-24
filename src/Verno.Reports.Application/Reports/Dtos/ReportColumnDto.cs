using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Verno.Reports.Models;

namespace Verno.Reports.Reports.Dtos
{
    [AutoMapFrom(typeof(ReportColumn))]
    public class ReportColumnDto : EntityDto
    {
        public string Field { get; set; }
        public string Title { get; set; }
        public string Format { get; set; }
        public string LinkUrl { get; set; }
        public string Formula { get; set; }
        public string Aggregate { get; set; }
        public bool Display { get; set; }
        public int SortOrder { get; set; }
        public int? ColumnGroupId { get; set; }
    }
}