using Abp.Application.Services.Dto;

namespace Verno.Identity.OrgUnits
{
    using Dto;

    public interface IOrgUnitAppService
    {
        ListResultOutput<OrgUnitDto> GetAll();
    }
}