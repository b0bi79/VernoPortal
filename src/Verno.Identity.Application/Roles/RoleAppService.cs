using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;

namespace Verno.Identity.Roles
{
    using Dto;

    public class RoleAppService : IdentityAppServiceBase, IRoleAppService
    {
        private readonly RoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;

        public RoleAppService(RoleManager roleManager, IPermissionManager permissionManager)
        {
            _roleManager = roleManager;
            _permissionManager = permissionManager;
        }

        #region Implementation of IRoleAppService

        /// <inheritdoc />
        public async Task UpdateRolePermissions(UpdateRolePermissionsInput input)
        {
            var role = await _roleManager.GetRoleByIdAsync(input.RoleId);
            var grantedPermissions = _permissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissionNames.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }

        /// <inheritdoc />
        public ListResultOutput<RoleDto> GetAll()
        {
            return new ListResultOutput<RoleDto>(
                _roleManager.Roles
                    .OrderBy(t => t.Name)
                    .ToList()
                    .MapTo<List<RoleDto>>()
            );
        }

        #endregion
    }
}