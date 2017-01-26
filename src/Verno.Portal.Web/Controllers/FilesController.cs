using System.IO;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Verno.Portal.Web.Modules.Returns;

namespace Verno.Portal.Web.Controllers
{
    public class FilesController : ReportsControllerBase
    {
        private readonly ReturnsAppService _returns;

        public FilesController(ReturnsAppService returns)
        {
            _returns = returns;
        }

        [HttpPost]
        [Route("api/services/app/returns/files")]
        [AbpMvcAuthorize(ReturnsPermissionNames.Documents_Returns_UploadFile)]
        public async Task<ReturnFileDto> ReturnForms([FromQuery]RasxodLink rasxodLink, IFormFile file)
        {
            return await _returns.UploadFile(rasxodLink, file);
        }
    }
}