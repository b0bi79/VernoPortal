using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Verno.Reports.Models;

namespace Verno.Reports.Reports.Dtos
{
    [AutoMapFrom(typeof(Report))]
    public class ReportDto : EntityDto
    {
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsFavorite { get; set; }

        public RepParameterDto[] Parameters { get; set; }

        public OutFormatDto[] OutFormats { get; set; }

        public ReportColumnDto[] Columns { get; set; }
    }
}
