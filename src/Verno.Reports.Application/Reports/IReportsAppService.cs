using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Verno.Reports.Executing;
using Verno.Reports.Reports.Dtos;

namespace Verno.Reports.Reports
{
    public interface IReportsAppService : IApplicationService
    {
        Task<ListResultDto<ReportDto>> GetAll();
        Task<ReportDto> Get(string normName);
        Task<ListValues> GetListValues(string reportName, string paramName, string filter = "");
        Task<FileStreamResult> Execute(string reportName, string outputFormat, RepParameterDto[] pars);
        Task<JObject> ExecuteJson(string reportName, RepParameterDto[] pars);
    }
}
