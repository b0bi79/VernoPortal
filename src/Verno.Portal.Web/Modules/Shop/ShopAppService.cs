using System;
using System.IO;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
//using Microsoft.Office.Interop.Word;
using Spire.Doc;
using Verno.ActionResults;
using Verno.Configuration;
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
        //private readonly NomenklaturaRepository _nomenklatura;
        private readonly KlientyRepository _klienty;

        public PrintAppService(ShopDbContext context, IOptions<AppSettings> appSettings, 
            /*NomenklaturaRepository nomenklatura, */KlientyRepository klienty)
        {
            _context = context;
            _appSettings = appSettings.Value;
            //_nomenklatura = nomenklatura;
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

        [Route("{shopNum}/production-calculator/excel")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production)]
        public async Task<ActionResult> GetProductionCalculatorToExcel(int shopNum)
        {
            var userShopNum = await GetUserShopNum();
            shopNum = userShopNum == 0 ? shopNum : userShopNum;
            if (shopNum == 0)
                throw new UserFriendlyException("Пользователю не присвоен номер магазина.");
            var datas = await _context.GetProductionCalculator(shopNum);
            using (var template = File.OpenRead(Path.Combine(_appSettings.XlReportTemplatesPath, "BakeCalculator.xlsx")))
            {
                var xlsxTemplate = new DotXLReport.XLTemplate();
                xlsxTemplate.Load(template);
                xlsxTemplate.AddVariable(datas, "table");
                xlsxTemplate.AddVariable(shopNum, "shopNum");

                return new MemoryStreamResult("BakeCalculator.xlsx", file=>xlsxTemplate.Generate(file));
            }
        }
        
        [HttpGet]
        [Route("{shopNum}/nomenklatura/{vidTovara}/sticker")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production)]
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
            //var tovar = await _nomenklatura.GetAsync(vidTovara);
            if (tovConf != null && tovConf.Etiketka != null/* && tovar != null*/)
            {
                var filepath = Path.Combine(ServPath, tovConf.Etiketka);
                if (File.Exists(filepath) && Path.GetExtension(filepath) == ".docx")
                {
                    Document document = new Document();
                    document.LoadFromFile(filepath);

                    document.Replace("{DateTime}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"), false, true);
                    document.Replace("{ProdAddr}", shop.Adres, false, true);

                    document.Protect(ProtectionType.AllowOnlyReading, "EFDE775C-56F6-47ED-8C79-0AC2C027D6B1");
                    return new MemoryStreamResult(tovConf.Etiketka, file => document.SaveToStream(file, FileFormat.Docx));
                }
            }
            return new NotFoundResult();
        }

        #endregion

        private string _servPath;
        public string ServPath => _servPath ?? (_servPath = _appSettings.PrintTemplatesPath);
    }
}
