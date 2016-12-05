using System;
using System.IO;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Linq;
using System.Net.Mime;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Verno.Identity.Users;
using Verno.Portal.Web.ActionResults;
using Verno.Portal.Web.Utils;

namespace Verno.Portal.Web.Modules.Print
{
    public interface IPrintAppService
    {
        Task<ListResultDto<PrintDto>> GetList(DateTime dfrom, DateTime dto, string filter);
        Task<ActionResult> File(int fileId);
    }

    [AbpAuthorize(PrintPermissionNames.Documents_Print)]
    public class PrintAppService : ApplicationService, IPrintAppService
    {
        private readonly PrintDbContext _context;
        private readonly HttpContext _httpContext;

        public PrintAppService(PrintDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _httpContext = contextAccessor.HttpContext;
        }

        public UserManager UserManager { get; set; }

        #region Implementation of IPrintAppService

        [HttpGet]
        [UnitOfWork(isTransactional: false)]
        [Route("api/services/app/print-forms/{dfrom:datetime}!{dto:datetime}")]
        public async Task<ListResultDto<PrintDto>> GetList(DateTime dfrom, DateTime dto, string filter)
        {
            int shopNum = await GetUserShopNum();
            var result =
                from d in _context.PrintDocs
                join f in _context.PrintDocForms on d.Id equals f.DokId
                join s in _context.Sklads on d.SklIst equals s.NomerSklada
                where d.DataNakl >= dfrom && d.DataNakl <= dto && f.Deleted == null
                select new PrintDto(d.Liniah, d.NomNakl, d.MagPol, d.DataNakl, f.ImahDok, d.SklIst, s.Postavthik /*d.SkladSrc.Platelqthik*/,
                    _httpContext.CreateUrl("/api/services/app/Print/File?fileId=" + f.Id));

            if (shopNum > 0)
                result = result.Where(d => d.ShopNum == shopNum);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var num = filter.AsInt(0);
                if (num > 0)
                    result = result.Where(doc => doc.ImahDok.Contains(filter) ||
                                                 doc.NomNakl.EndsWith(filter) ||
                                                 doc.SrcWhId == num ||
                                                 doc.ShopNum == num);
                else
                    result = result.Where(doc => doc.ImahDok.Contains(filter) ||
                                                 doc.NomNakl.EndsWith(filter));
            }
            return new ListResultDto<PrintDto>(await result.Take(500).ToListAsync());
        }

        [HttpGet]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(PrintPermissionNames.Documents_Print_GetFile)]
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