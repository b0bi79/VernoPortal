using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Verno.Identity.Roles;
using Verno.Reports.Authorization;
using Verno.Reports.DataSource;
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
        private readonly IRepository<ReportOutFormat> _reportFormatRepository;
        private readonly IReportGeneratorFactory _generatorFactory;

        private static OutFormat JsonOutput;

        public ReportsAppService(IRepository<Report> reportRepository, IRepository<RolePermission> rolePermissions, 
            IRepository<UserPermission> userPermissions, IRepository<OutFormat, string> outFormatRepository,
            IRepository<ReportOutFormat> reportFormatRepository, IReportGeneratorFactory generatorFactory)
        {
            _reportRepository = reportRepository;
            _rolePermissions = rolePermissions;
            _userPermissions = userPermissions;
            _reportFormatRepository = reportFormatRepository;
            _generatorFactory = generatorFactory;

            if (JsonOutput == null)
                JsonOutput = outFormatRepository.Get("JSON");
        }

        [Route("")]
        public async Task<ListResultDto<ReportDto>> GetAll()
        {
            var user = await GetCurrentUserAsync();
            var reports = await _reportRepository.GetAllIncluding(r => r.Favorites).ToListAsync();
            var reportList = reports.Select(r =>
            {
                var dto = ObjectMapper.Map<ReportDto>(r);
                dto.IsFavorite = r.Favorites.Any(f => f.UserId == user.Id);
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

        [Route("{normName}")]
        public async Task<ReportDto> Get(string normName)
        {
            var user = await GetCurrentUserAsync();
            var report = await _reportRepository
                .GetAllIncluding(r => r.Favorites, r => r.Parameters, r => r.Columns, r => r.Connection)
                .Include(r => r.ReportOutFormats).ThenInclude(x => x.OutFormat)
                .SingleAsync(r => r.NormalizedName == normName);

            var dto = ObjectMapper.Map<ReportDto>(report);
            dto.IsFavorite = report.Favorites.Any(f => f.UserId == user.Id);
            dto.OutFormats = ObjectMapper.Map<OutFormatDto[]>(report.ReportOutFormats.Select(x => x.OutFormat));
            dto.Parameters = report.Parameters.Select(p =>
            {
                var pardto = ObjectMapper.Map<RepParameterDto>(p);
                bool lazy;
                pardto.Values = ListValues.Parse(p, report, out lazy);
                pardto.Lazy = lazy;
                return pardto;
            }).ToArray();
            return dto;
        }

        [Route("{reportName}/params/{paramName}/values")]
        public async Task<ListValues> GetListValues(string reportName, string paramName, string filter = "")
        {
            var report = await _reportRepository
                .GetAllIncluding(r => r.Parameters, r => r.Connection)
                .SingleAsync(r => r.NormalizedName == reportName);

            var param = report.Parameters.Single(x => x.Name == paramName);
            var listValues = ListValues.Parse(param, report);
            var result = new ListValues(param);
            result.AddRange(listValues.Where(x=>x.Name.StartsWith(filter)));
            return result;
        }

        [HttpPost]
        [Route("{reportName}/execute/{outputFormat}")]
        // TODO объеденить методы, возвращать ActionResult
        public async Task<FileStreamResult> Execute(string reportName, string outputFormat, RepParameterDto[] pars)
        {
            var report = await BuildReport(reportName, pars);

            var format = await FindFormat(report.Id, outputFormat)
                         ?? new ReportOutFormat(JsonOutput.Id) {Report = report, OutFormat = JsonOutput};

            var result = GenerateReport(report, format);
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(result.OutFileName, out contentType);

            return new FileStreamResult(result.OutStream, contentType) { FileDownloadName = result.OutFileName };
        }

        [HttpPost]
        [Route("{reportName}/execute/json")]
        public async Task<JObject> ExecuteJson(string reportName, RepParameterDto[] pars)
        {
            var report = await BuildReport(reportName, pars);
            var format = new ReportOutFormat(JsonOutput.Id) { Report = report, OutFormat = JsonOutput };

            var result = GenerateReport(report, format);
            result.OutStream.Position = 0;
            var streamReader = new StreamReader(result.OutStream);
            var json = streamReader.ReadToEnd();
            return JObject.Parse(json);
        }

        private async Task<Report> BuildReport(string reportName, RepParameterDto[] pars)
        {
            var user = await GetCurrentUserAsync();
            var userClaims = await UserManager.GetClaimsAsync(user);
            var report = await _reportRepository
                .GetAllIncluding(r => r.Parameters, r => r.Columns, r => r.Connection, r => r.Results)
                .SingleAsync(r => r.NormalizedName == reportName);

            // Заполнение параметров отчёта
            foreach (var par in report.Parameters)
            {
                var val = Array.Find(pars, i => i.Id == par.Id);
                if (val != null) par.Value = val.Value;
            }
            report.Parameters.Add(new RepParameter
            {
                DisplayText = "Пользователь",
                DisplayType = DisplayType.TextBox,
                Name = "_user",
                ValueType = "string",
                Value = user.UserName
            });
            var claims = userClaims
                .Where(c => !c.Type.StartsWith("http:"))
                .GroupBy(c => c.Type, c => c.Value)
                .ToDictionary(claim => claim.Key.Replace(".", "_"), claim => string.Join(", ", claim)).ToList();
            foreach (var kv in claims)
            {
                report.Parameters.Add(new RepParameter
                {
                    DisplayText = kv.Key,
                    DisplayType = DisplayType.TextBox,
                    Name = "_" + kv.Key,
                    ValueType = "string",
                    Value = kv.Value
                });
            }
            return report;
        }

        private Executing.ReportResult GenerateReport(Report report, ReportOutFormat format)
        {
            var formatMapping = FormatMapping.GetOutputFormat(format.OutFormat);
            string outFileName = $"{report.Name}_{DateTime.Now:yy-MM-dd HH-mm}{formatMapping.FileExtension}";

            var gen = _generatorFactory.Create(format);
            var stream = new MemoryStream();
            gen.Generate(report, stream);

            return new Executing.ReportResult(outFileName, stream);
        }

        private Task<ReportOutFormat> FindFormat(int reportId, string outputFormat)
        {
            return _reportFormatRepository
                     .GetAllIncluding(x => x.OutFormat)
                     .FirstOrDefaultAsync(i => i.OutFormatId == outputFormat && i.ReportId == reportId);
        }
    }
}