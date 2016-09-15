using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Verno.Identity.Organizations;

namespace Verno.Identity.OrgUnits.Dto
{
    [AutoMap(typeof(OrgUnit))]
    public class OrgUnitDto: EntityDto
    {
        public string Name { get; set; }
        public string OrgUnitType { get; set; }
        public int? ParentUnitId { get; set; }
        public string Code { get; set; }
    }
}