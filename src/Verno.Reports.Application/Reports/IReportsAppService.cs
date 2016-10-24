using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Verno.Reports.Reports.Dtos;

namespace Verno.Reports.Reports
{
    public interface IReportsAppService : IApplicationService
    {
        Task<ListResultDto<ReportDto>> GetAll();
    }
}
