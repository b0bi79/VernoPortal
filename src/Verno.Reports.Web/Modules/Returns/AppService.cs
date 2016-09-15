using System;
using System.IO;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Linq;
using System.Net.Mime;
using Abp.Authorization;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Verno.Identity.Users;
using Verno.Reports.Web.ActionResults;

namespace Verno.Reports.Web.Modules.Returns
{
    public interface IReturnsAppService
    {
        Task<ListResultOutput<ReturnDto>> GetList(DateTime dfrom, DateTime dto);
        Task<ActionResult> File(int fileId);
    }

    [AbpAuthorize("Documents.Returns")]
    public class ReturnsAppService : ApplicationService, IReturnsAppService
    {
        private readonly ReturnsDbContext _context;
        private readonly string _root;

        public ReturnsAppService(ReturnsDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _root = contextAccessor.HttpContext.Request.PathBase;
        }

        public UserManager UserManager { get; set; }

        #region Implementation of IReturnAppService

        [HttpGet]
        [Route("returns/{dfrom:datetime}!{dto:datetime}")]
        public async Task<ListResultOutput<ReturnDto>> GetList(DateTime dfrom, DateTime dto)
        {
            int shopNum = await GetUserShopNum();
            var result =
                from d in _context.PrintDocs
                join f in _context.PrintDocForms on d.Id equals f.DokId
                join s in _context.Sklads on d.SklIst equals s.NomerSklada
                where d.DataNakl >= dfrom && d.DataNakl <= dto &&
                      (d.MagPol == shopNum || shopNum == 0) && f.Deleted == null
                select new ReturnDto(d.Liniah, d.NomNakl, d.DataNakl, f.ImahDok, d.SklIst, s.Postavthik /*d.SkladSrc.Platelqthik*/,
                    _root + "/api/services/app/Print/File?fileId=" + f.Id);
            return new ListResultOutput<ReturnDto>(await result.ToListAsync());
            /*var result = new [] {
                new PrintDto(6, "131/s1", DateTime.Today.AddDays(-2), "Акт инвентаризации", _root + "/api/services/app/Print/File?fileId=8"),
                new PrintDto(5, "141/u3", DateTime.Today.AddDays(-1), "Акт списания", _root + "/api/services/app/Print/File?fileId=8"),
                new PrintDto(5, "185/u12", DateTime.Today, "Акт списания", _root + "/api/services/app/Print/File?fileId=8"),
            }.Where(d=> d.DataNakl >= dfrom && d.DataNakl <= dto);
            return new ListResultOutput<PrintDto>(result.ToList());*/
        }

        [HttpGet]
        [AbpAuthorize("Documents.Returns.GetFile")]
        public async Task<ActionResult> File(int fileId)
        {
            var resultFile = await _context.PrintDocForms.FirstOrDefaultAsync(f => f.Id == fileId);
            if (resultFile != null)
            {
                var filepath = Path.Combine(ServPath, resultFile.Putq);
                if (System.IO.File.Exists(filepath))
                {
                    string contentType;
                    new FileExtensionContentTypeProvider().TryGetContentType(filepath, out contentType);
                    return new PhysicalFileResultWithContentDisposition(filepath, contentType ?? "application/octet-stream", new ContentDisposition()
                    {
                        Inline = true,
                        FileName = Path.GetFileName(resultFile.Putq)
                    });
                }
            }
            return new NotFoundResult();
        }

        private string _servPath;
        public string ServPath
        {
            get { return _servPath ?? (_servPath = _context.PutqServ.Select(s => s.Putq).FirstOrDefault()); }
        }

        #endregion

        private async Task<int> GetUserShopNum()
        {
            var user = await GetCurrentUserAsync();
            var claims = await UserManager.GetClaimAsync(user, UserClaimTypes.ShopNum);
            if (claims == null)
                return 0;
            return int.Parse(claims.Value);
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }
    }
}