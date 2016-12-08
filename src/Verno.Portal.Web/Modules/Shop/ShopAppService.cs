using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
//using Microsoft.Office.Interop.Word;
using Spire.Doc;
using Verno.ActionResults;
using Verno.Configuration;
using Verno.Filters;
using Verno.Identity.Users;
using Verno.ShInfo.EntityFrameworkCore.Repositories;

namespace Verno.Portal.Web.Modules.Shop
{
    public interface IShopAppService
    {
        Task<ListResultDto<ProductionCalc>> GetProductionCalculator(int shopNum);
    }

    [Route("api/services/app/shop")]
    [AbpAuthorize(ShopPermissionNames.Documents_Shop)]
    public class PrintAppService : ApplicationService, IShopAppService
    {
        private readonly ShopDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly NomenklaturaRepository _nomenklatura;
        private readonly KlientyRepository _klienty;

        public PrintAppService(ShopDbContext context, IOptions<AppSettings> appSettings, 
            NomenklaturaRepository nomenklatura, KlientyRepository klienty)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _nomenklatura = nomenklatura;
            _klienty = klienty;
        }

        public UserManager UserManager { get; set; }


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

        #region Implementation of IShopAppService

        [Route("{shopNum}/production-calculator")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production)]
        public async Task<ListResultDto<ProductionCalc>> GetProductionCalculator(int shopNum)
        {
            var userShopNum = await GetUserShopNum();
            shopNum = userShopNum == 0 ? shopNum : userShopNum;
            if (shopNum == 0)
                throw new UserFriendlyException("Пользователю не присвоен номер магазина.");
            return new ListResultDto<ProductionCalc>(await _context.GetProductionCalculator(shopNum));
        }


        [HttpGet]
        [Route("{shopNum}/nomenklatura/{vidTovara}/sticker")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production)]
        [DeleteFile]
        public async Task<ActionResult> StickerFile(int shopNum, int vidTovara)
        {
            var userShopNum = await GetUserShopNum();
            shopNum = userShopNum == 0 ? shopNum : userShopNum;
            if (shopNum == 0)
                throw new UserFriendlyException("Пользователю не присвоен номер магазина.");

            var shop = await _klienty.GetByShopNumAsync(shopNum);
            if (shop == null)
                throw new UserFriendlyException($"Не найден магазин {shopNum}.");

            var tovConf = await _context.KonfProizvMag.FirstOrDefaultAsync(x => x.VidTovara == vidTovara);
            var tovar = await _nomenklatura.GetAsync(vidTovara);
            if (tovConf != null && tovar != null)
            {
                var outfile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                var filepath = Path.Combine(ServPath, tovConf.Etiketka);
                if (System.IO.File.Exists(filepath) && Path.HasExtension(".docx"))
                {
                    Document document = new Document();
                    document.LoadFromFile(filepath);

                    document.Replace("{DateTime}", DateTime.Now.ToString("d t"), false, true);
                    document.Replace("{ProdAddr}", shop.Adres, false, true);

                    document.SaveToFile(outfile, FileFormat.PDF);

                    /*var appWord = new Application();
                    var docWord = appWord.Documents.Open(filepath, ReadOnly: false, Visible: false);
                    try
                    {
                        docWord.Activate();
                        FindAndReplace(appWord, "{DateTime}", DateTime.Now.ToString("d t"));
                        FindAndReplace(appWord, "{ProdAddr}", shop.Adres);
                        docWord.ExportAsFixedFormat(outfile, WdExportFormat.wdExportFormatPDF);
                    }
                    finally
                    {
                        docWord.Close();
                        appWord.Quit();
                    }*/

                    return new PhysicalFileResultWithContentDisposition(outfile, "application/pdf", new ContentDisposition()
                    {
                        Inline = true,
                        FileName = Path.GetFileName(outfile)
                    });
                }
            }
            return new NotFoundResult();
        }
        #endregion

        /*private void FindAndReplace(Application doc, object findText, object replaceWithText)
        {
            //options
            object matchCase = false, matchWholeWord = true, matchWildCards = false, matchSoundsLike = false,
                matchAllWordForms = false, forward = true, format = false, matchKashida = false, matchDiacritics = false,
                matchAlefHamza = false, matchControl = false, read_only = false, visible = true, replace = 2, wrap = 1;
            //execute find and replace
            doc.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
        }*/

        private string _servPath;
        public string ServPath => _servPath ?? (_servPath = _appSettings.PrintTemplatesPath);
    }
}
