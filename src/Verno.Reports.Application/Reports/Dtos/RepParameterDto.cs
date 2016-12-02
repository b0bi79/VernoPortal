using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Verno.Reports.Executing;
using Verno.Reports.Models;

namespace Verno.Reports.Reports.Dtos
{
    [AutoMap(typeof(RepParameter))]
    public class RepParameterDto : EntityDto
    {
        public string Name { get; set; }
        public string DisplayText { get; set; }
        public string ValueType { get; set; }
        public string DisplayType { get; set; }
        public bool IsRequired { get; set; }
        public string ValueFormat { get; set; }
        public string Value { get; set; }
        public ListValues Values { get; set; }
        public string HelpText { get; set; }
        public bool Lazy { get; set; }
    }
}