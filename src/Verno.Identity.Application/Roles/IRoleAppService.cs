using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Verno.Identity.Roles.Dto;

namespace Verno.Identity.Roles
{
    public interface IRoleAppService : IApplicationService
    {
        Task UpdateRolePermissions(UpdateRolePermissionsInput input);
        ListResultOutput<RoleDto> GetAll();
    }
}
