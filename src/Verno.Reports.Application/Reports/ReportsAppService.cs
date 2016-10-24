using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Verno.Identity.Roles;
using Verno.Reports.Authorization;
using Verno.Reports.Executing;
using Verno.Reports.Models;
using Verno.Reports.Reports.Dtos;

namespace Verno.Reports.Reports
{
    [Route("api/services/reports")]
    [AbpAuthorize(PermissionNames.Reports)]
    public class ReportsAppService : IdentityAppServiceBase, IReportsAppService
    {
        private readonly IRepository<Report> _reportRepository;
        private readonly IRepository<RolePermission> _rolePermissions;
        private readonly IRepository<UserPermission> _userPermissions;

        public ReportsAppService(IRepository<Report> reportRepository, IRepository<RolePermission> rolePermissions, IRepository<UserPermission> userPermissions)
        {
            _reportRepository = reportRepository;
            _rolePermissions = rolePermissions;
            _userPermissions = userPermissions;
        }

        [HttpGet]
        [Route("")]
        public async Task<ListResultDto<ReportDto>> GetAll()
        {
            var user = await GetCurrentUserAsync();
            var reports = await _reportRepository.GetAllIncluding(r => r.OutFormats, r => r.Favorites, r => r.Parameters, r => r.Columns).ToListAsync();
            var reportList = reports.Select(r =>
            {
                var dto = ObjectMapper.Map<ReportDto>(r);
                dto.IsFavorite = r.Favorites.Any(f => f.UserId == user.Id);
                dto.Columns = ObjectMapper.Map<ReportColumnDto[]>(r.Columns);
                dto.OutFormats = ObjectMapper.Map<OutFormatDto[]>(r.OutFormats);
                dto.Parameters = r.Parameters.Select(p =>
                {
                    var pardto = ObjectMapper.Map<RepParameterDto>(p);
                    pardto.Values = ListValues.Parse(p, r);
                    return pardto;
                }).ToArray();
                return dto;
            }).ToList();

            var userRoles = await UserManager.GetRolesAsync(user);
            if (userRoles.Contains(StaticRoleNames.Admin))
                return new ListResultDto<ReportDto>(reportList.ToList());

            var filteredReports = (
                                      from r in userRoles
                                      join p in await _rolePermissions.GetAllListAsync() on r equals p.Role
                                      join rep in reportList on p.ReportId equals rep.Id
                                      select rep)
                               .Union(
                                      from p in await _userPermissions.GetAll().Where(i => i.UserId == user.Id).ToListAsync()
                                      join rep in reportList on p.ReportId equals rep.Id
                                      select rep);

            return new ListResultDto<ReportDto>(filteredReports.ToList());
        }
    }
}