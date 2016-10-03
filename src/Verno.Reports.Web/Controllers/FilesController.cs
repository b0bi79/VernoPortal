using System.IO;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Verno.Reports.Web.Modules.Returns;

namespace Verno.Reports.Web.Controllers
{
    public class FilesController : ReportsControllerBase
    {
        private readonly ReturnsAppService _returns;

        public FilesController(ReturnsAppService returns)
        {
            _returns = returns;
        }

        [HttpPost]
        [Route("api/services/app/returns/{rasxod}/files")]
        [AbpMvcAuthorize("Documents.Returns.UploadFile")]
        public async Task<ReturnFileDto> ReturnForms(int rasxod, IFormFile file)
        {
            return await _returns.UploadFile(rasxod, file);
        }
    }
}