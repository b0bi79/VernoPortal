using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Verno.Identity.OrgUnits
{
    using Organizations;
    using Dto;

    public class OrgUnitAppService : IdentityAppServiceBase, IOrgUnitAppService
    {
        private readonly OrgUnitManager _orgUnitManager;

        public OrgUnitAppService(OrgUnitManager orgUnitManager)
        {
            _orgUnitManager = orgUnitManager;
        }

        public ListResultOutput<OrgUnitDto> GetAll()
        {
            return new ListResultOutput<OrgUnitDto>(
                _orgUnitManager.OrgUnits
                    .OrderBy(t => t.Name)
                    .ToList()
                    .MapTo<List<OrgUnitDto>>()
            );
        }
    }
}